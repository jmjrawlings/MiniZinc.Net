namespace Make;

using System.Text;
using System.Text.Json.Nodes;
using CommunityToolkit.Diagnostics;
using LibMiniZinc.Tests;
using MiniZinc.Build;
using static Console;

public static class ParseLibMiniZincTests
{
    public static Task Run()
    {
        WriteLine("Parsing libminiznc test suite");
        FileInfo input = Repo.TestSpecYaml;
        FileInfo output = Repo.TestSpecJson;

        WriteLine($"Parsing {input.FullName}");
        var spec = ParseYaml(input);

        WriteLine($"Writing to {output.FullName}");
        TestSpec.ToJsonFile(spec, output);
        return Task.CompletedTask;
    }

    public static TestSpec ParseYaml(FileInfo file)
    {
        var spec = new TestSpec { TestCases = new(), TestSuites = new() };
        var dir = file.Directory!;
        var modelPaths = new HashSet<string>();
        var specNode = Yaml.ParseFile<JsonObject>(file);
        Guard.IsNotNull(specNode);

        foreach (var (suiteName, suiteJson) in specNode)
        {
            var json = suiteJson!.AsObject();
            var strict = json["strict"]?.GetValue<bool>();
            var options = json["options"]?.AsObject();
            var solvers =
                json["solvers"]?.AsArray().Select(x => x!.GetValue<string>()).ToList()
                ?? new List<string>();
            var includes = json["includes"]!.AsArray().Select(x => x!.GetValue<string>()).ToList();
            var suite = new TestSuite
            {
                Name = suiteName,
                Strict = strict,
                Options = options,
                IncludeGlobs = includes,
                Solvers = solvers
            };
            spec.TestSuites.Add(suite);

            foreach (var glob in suite.IncludeGlobs)
            {
                foreach (var model in dir.EnumerateFiles(glob, SearchOption.AllDirectories))
                {
                    if (model.Extension != ".mzn")
                        continue;

                    var path = Path.GetRelativePath(dir.FullName, model.FullName)
                        .Replace('\\', '/');
                    modelPaths.Add(path);
                }
            }
        }

        foreach (var modelPath in modelPaths)
        {
            var model = new FileInfo(Path.Join(dir.FullName, modelPath));
            foreach (var yaml in ParseTestCaseYaml(model))
            {
                var node = Yaml.ParseString<JsonObject>(yaml);
                if (node is null)
                    continue;
                var testCase = ParseTestCase(modelPath, node);
                spec.TestCases.Add(testCase);
            }
        }

        return spec;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static TestCase ParseTestCase(string path, JsonObject node)
    {
        var solvers = node["solvers"]?.ToNonEmptyList<string>();
        var options = (node["solve_options"] ?? node["options"])?.AsObject();
        var allSolutions = options?.Pop("all_solutions").TryGetValue<bool>() ?? false;
        var test_type = node["type"]?.GetValue<string>();
        var inputFiles = node["extra_files"]?.ToNonEmptyList<string>();
        var results = node["expected"].AsListOf<JsonObject>();
        var checkAgainst = node["check_against"]?.ToNonEmptyList<string>();

        var testCase = new TestCase
        {
            Solvers = solvers,
            Options = options?.Count > 0 ? options : null,
            InputFiles = inputFiles,
            CheckAgainstSolvers = checkAgainst,
            Path = path,
            Type = allSolutions ? TestType.AllSolutions : TestType.AnySolution
        };

        foreach (var result in results)
        {
            var status = result.TryGetValue<string>("status");
            var tag = GetTag(result);
            if (tag is Yaml.Tag.Error)
            {
                testCase.ErrorRegex = result.TryGetValue<string>("regex");
                testCase.ErrorMessage = result.TryGetValue<string>("message");
                testCase.Type = result.TryGetValue<string>("type") switch
                {
                    "AssertionError" => TestType.AssertionError,
                    "EvaluationError" => TestType.EvaluationError,
                    "MiniZincError" => TestType.MiniZincError,
                    "TypeError" => TestType.TypeError,
                    "SyntaxError" => TestType.SyntaxError,
                    _ => TestType.Error
                };
            }
            else if (result.Pop("solution") is { } x)
            {
                if (allSolutions)
                {
                    foreach (var solutionNode in x.AsListOf<JsonObject>())
                    {
                        var sol = ParseSolution(solutionNode);
                        testCase.Solutions ??= new List<JsonObject>();
                        testCase.Solutions.Add(sol);
                    }
                }
                else
                {
                    var solutionNode = x.AsObject();
                    if (solutionNode.Count > 0)
                    {
                        var sol = ParseSolution(solutionNode);
                        if (status is "OPTIMAL_SOLUTION")
                            testCase.Type = TestType.Optimise;
                        testCase.Solutions ??= new List<JsonObject>();
                        testCase.Solutions.Add(sol);
                    }
                    else
                    {
                        testCase.Type = TestType.Satisfy;
                    }
                }
            }
            else if (result.TryGetValue<string>("flatzinc") is { } fzn)
            {
                testCase.Type = TestType.Compile;
                testCase.OutputFiles ??= new List<string>();
                testCase.OutputFiles.Add(fzn);
            }
            else if (result.TryGetValue<string>("outputmodel") is { } ozn)
            {
                testCase.Type = TestType.OutputModel;
                testCase.OutputFiles ??= new List<string>();
                testCase.OutputFiles.Add(ozn);
            }
            else if (status is "UNSATISFIABLE")
            {
                testCase.Type = TestType.Unsatisfiable;
                break;
            }
            else if (status is "SATISFIED")
            {
                testCase.Type = TestType.Satisfy;
                break;
            }
            else if (result.Count == 0)
            {
                testCase.Type = TestType.Satisfy;
            }
            else { }
        }

        var nsols = testCase.Solutions?.Count ?? 0;
        return testCase;
    }

    private static JsonObject ParseSolution(JsonObject node)
    {
        var tag = GetTag(node);
        return node;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static IEnumerable<string> ParseTestCaseYaml(FileInfo file)
    {
        var sb = new StringBuilder();
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);
        char c;

        if (!Skip('/'))
            yield break;

        preamble:
        Read();
        if (c is '\n' or '*' or '-')
            goto preamble;
        if (char.IsWhiteSpace(c))
            goto preamble;

        contents:
        sb.Append(c);
        if (reader.EndOfStream)
            goto fin;
        Read();

        if (c is '*')
            if (Skip('*'))
                if (Skip('*'))
                    if (Skip('/'))
                        goto fin;
        goto contents;

        fin:

        var yamlString = sb.ToString();
        var testStrings = yamlString.Split(
            "---",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        foreach (var s in testStrings)
        {
            yield return s;
        }
        yield break;

        bool Skip(char c)
        {
            if (reader.Peek() != c)
                return false;

            Read();
            return true;
        }

        void Read()
        {
            c = (char)reader.Read();
        }
    }

    private static Yaml.Tag? GetTag(JsonNode? node)
    {
        var tag = node.Match(o =>
        {
            if (o.ContainsKey(Yaml.TAG))
            {
                var tagString = o[Yaml.TAG]!.GetValue<string>();
                o.Remove(Yaml.TAG);
                Enum.TryParse(tagString, out Yaml.Tag tag);
                return tag;
            }
            return default;
        });
        return tag;
    }
}
