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
            foreach (var testCase in ParseTestCasesFromModelComments(model))
            {
                testCase.Path = modelPath;
                spec.TestCases.Add(testCase);
            }
        }

        return spec;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static TestCase ParseTestCasesFromModelComments(JsonObject json)
    {
        var solvers = json["solvers"]?.ToNonEmptyList<string>();
        var solveOptions = json["options"]?.AsObject();
        var type = json["type"]?.GetValue<string>();
        var extraFiles = json["extra_files"]?.ToNonEmptyList<string>();
        var expected = json["expected"];

        var testCase = new TestCase
        {
            Solvers = solvers,
            SolveOptions = solveOptions,
            ExtraFiles = extraFiles
        };

        foreach (var result in ParseTestResults(type, expected))
            testCase.Results.Add(result);

        return testCase;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static IEnumerable<TestCase> ParseTestCasesFromModelComments(FileInfo file)
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
            var json = Yaml.ParseString<JsonObject>(s);
            if (json is null)
            {
                Console.WriteLine($"Could not parse yaml for {file.FullName}");
                continue;
            }

            var @case = ParseTestCasesFromModelComments(json);
            yield return @case;
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

    public static IEnumerable<TestResult> ParseTestResults(string? type, JsonNode? node)
    {
        if (node is null)
            yield break;

        if (node is JsonArray arr)
        {
            foreach (var obj in arr)
            {
                foreach (var res in ParseTestResults(type, obj))
                {
                    yield return res;
                }
            }

            yield break;
        }

        var solution = node["solution"];
        var status = node.GetString("status");
        string? tag = node.GetString(Yaml.TAG);
        var result = (type, tag, status) switch
        {
            (_, Yaml.TAG_ERROR, _) => ParseErrorResult(node),
            (_, Yaml.TAG_RESULT, Yaml.SATISFIED) => ParseSatisfyResult(node),
            (_, Yaml.TAG_RESULT, Yaml.UNSATISFIABLE) => ParseUnsatisfiableResult(node),
            (_, Yaml.TAG_RESULT, _) => ParseSolutionResult(solution),
            ("compile", _, _) => ParseCompileResult(node),
            ("output-model", _, _) => ParseOutputResult(node),
            _ => throw new Exception()
        };

        yield return result;
    }

    private static TestResult ParseOutputResult(JsonNode node)
    {
        var ozn = node.GetStringExn(Yaml.OUTPUT_MODEL);
        var result = new TestResult
        {
            Type = TestResultType.Compile,
            Files = new() { ozn }
        };
        return result;
    }

    private static TestResult ParseCompileResult(JsonNode node)
    {
        var fzn = node.GetStringExn(Yaml.FLATZINC);
        var result = new TestResult
        {
            Type = TestResultType.Compile,
            Files = new() { fzn! }
        };
        return result;
    }

    private static TestResult ParseSolutionResult(JsonNode? node)
    {
        var result = new TestResult { Type = TestResultType.Solve, Solution = node };
        return result;
    }

    private static TestResult ParseSatisfyResult(JsonNode node)
    {
        var result = new TestResult { Type = TestResultType.Satisfy };
        return result;
    }

    private static TestResult ParseUnsatisfiableResult(JsonNode node)
    {
        var result = new TestResult { Type = TestResultType.Unsatisfiable };
        return result;
    }

    private static TestResult ParseErrorResult(JsonNode node)
    {
        var result = new TestResult { Type = TestResultType.Error };
        return result;
    }
}
