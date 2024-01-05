namespace MiniZinc.Net.Tests;

using System.Text;
using System.Text.Json.Nodes;
using CommunityToolkit.Diagnostics;

public sealed record TestSpec
{
    public List<TestSuite> TestSuites { get; } = new();

    public List<TestCase> TestCases { get; } = new();

    public static TestSpec ParseTestSpecFromJson(FileInfo file)
    {
        var spec = Json.DeserializeFromFile<TestSpec>(file);
        return spec;
    }

    public static TestSpec ParseTestSpecFromYaml(FileInfo file)
    {
        var spec = new TestSpec();
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

                    var path = Path.GetRelativePath(dir.FullName, model.FullName);
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
                {
                    Console.WriteLine($"Could not parse test case for {modelPath}");
                    continue;
                }

                var testCase = ParseTestCase(node);
                testCase.Path = modelPath;
                spec.TestCases.Add(testCase);
            }
        }

        return spec;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static TestCase ParseTestCase(JsonObject node)
    {
        var solvers = node["solvers"]?.ToNonEmptyList<string>();
        var options = node["solve_options"]?.AsObject();
        var allSolutions = options?.TryGetValue<bool>("all_solutions") ?? false;
        var type = node["type"]?.GetValue<string>();
        var inputFiles = node["extra_files"]?.ToNonEmptyList<string>();
        var expected = node["expected"];
        var status = (expected as JsonObject)?.TryGetValue<string>("status");
        var testCase = new TestCase
        {
            Solvers = solvers,
            SolveOptions = options,
            InputFiles = inputFiles
        };

        string? tag = GetTag(expected);

        if (expected is JsonArray arr) { }
        else if (tag is Yaml.TAG_ERROR)
        {
            testCase.Type = TestType.Error;
        }
        else if (tag is Yaml.TAG_RESULT)
        {
            if (status is Yaml.UNSATISFIABLE)
                testCase.Type = TestType.Unsatisfiable;
            else { }
        }
        else if (type is "compile")
        {
            var fzn = expected.GetValue<string>(Yaml.FLATZINC);
            testCase.Type = TestType.Compile;
            testCase.OutputFiles = new() { fzn! };
        }
        else if (type is "output-model")
        {
            var ozn = expected.GetValue<string>(Yaml.OUTPUT_MODEL);
            testCase.Type = TestType.Compile;
            testCase.OutputFiles = new() { ozn };
        }
        else
        {
            throw new Exception("Unhandled path");
        }

        return testCase;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static IEnumerable<string> ParseTestCaseYaml(FileInfo file)
    {
        var sb = new StringBuilder();
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);

        const char EOF = '\uffff';
        char c;
        int i = -1;

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
            i++;
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
        node.Visit(o =>
        {
            if (o.ContainsKey(Yaml.TAG))
            {
                o.Remove(Yaml.TAG);
            }
        });
    }
}
