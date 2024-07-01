namespace LibMiniZinc.Tests;

using System.Text;
using System.Text.Json.Nodes;
using CommunityToolkit.Diagnostics;

public static class Spec
{
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
            string? tag = GetTag(result);

            if (tag is Yaml.TAG_ERROR)
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
                var sols = x.AsListOf<JsonObject>();
                foreach (var sol in sols)
                {
                    StripTags(sol);
                    if (sol.Count == 0)
                    {
                        testCase.Type = TestType.Satisfy;
                        break;
                    }
                    var solution = ParseSolutionVariables(sol);
                    if (status is Yaml.OPTIMAL)
                        testCase.Type = TestType.Optimise;

                    testCase.Solutions ??= new List<string>();
                    testCase.Solutions.Add(solution);
                }
            }
            else if (result.TryGetValue<string>(Yaml.FLATZINC) is { } fzn)
            {
                testCase.Type = TestType.Compile;
                testCase.OutputFiles ??= new List<string>();
                testCase.OutputFiles.Add(fzn);
            }
            else if (result.TryGetValue<string>(Yaml.OUTPUT_MODEL) is { } ozn)
            {
                testCase.Type = TestType.OutputModel;
                testCase.OutputFiles ??= new List<string>();
                testCase.OutputFiles.Add(ozn);
            }
            else if (status is Yaml.UNSATISFIABLE)
            {
                testCase.Type = TestType.Unsatisfiable;
                break;
            }
            else if (status is Yaml.SATISFIED)
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

    /// <summary>
    /// Parse solution variables as a dzn string
    /// </summary>
    private static string ParseSolutionVariables(JsonObject sol)
    {
        var sb = new StringBuilder();

        foreach (var kv in sol)
        {
            var name = kv.Key;
            var val = kv.Value;
            sb.Append(name);
            sb.Append('=');
            ParseSolutionValue(val!, sb);
            sb.Append(';');
        }

        var dzn = sb.ToString();
        return dzn;
    }

    private static void ParseSolutionValue(JsonNode n, StringBuilder sb)
    {
        switch (n)
        {
            case null:
                sb.Append("<>");
                break;

            case JsonArray { Count: 0 }:
                sb.Append("[]");
                break;

            case JsonArray x when x[0] is JsonArray:
                break;

            case JsonArray x:
                break;

            case JsonObject x:
                sb.Append('(');
                int i = 0;
                foreach (var kv in x)
                {
                    var field = kv.Key;
                    var val = kv.Value!;
                    if (i++ > 1)
                        sb.Append(',');
                    sb.Append(field);
                    sb.Append(':');
                    ParseSolutionValue(val, sb);
                }
                sb.Append(')');
                break;

            case JsonValue x:
                sb.Append(x);
                break;
        }
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

    private static string? GetTag(JsonNode? node)
    {
        var tag = node.Match(o =>
        {
            if (o.ContainsKey(Yaml.TAG))
            {
                var tag = o[Yaml.TAG]!.GetValue<string>();
                o.Remove(Yaml.TAG);
                return tag;
            }

            return null;
        });
        return tag;
    }

    private static void StripTags(JsonNode? node)
    {
        node.Walk(o =>
        {
            o.Pop(Yaml.TAG);
        });
    }
}
