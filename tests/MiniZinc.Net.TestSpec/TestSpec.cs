namespace MiniZinc.Net.Tests;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;

public static class TestSpec
{
    public static List<TestSuite> ParseTestSuitesFromJson(FileInfo file)
    {
        var text = file.OpenText().ReadToEnd();
        var suites = JsonSerializer.Deserialize<List<TestSuite>>(text, JsonSerializerOptions);
        Guard.IsNotNull(suites);
        return suites;
    }

    public static string WriteToJsonString(object obj)
    {
        var json = JsonSerializer.Serialize(obj, JsonSerializerOptions);
        return json;
    }

    public static FileInfo WriteJsonToFile(object obj, FileInfo file)
    {
        var json = WriteToJsonString(obj);
        File.WriteAllText(file.FullName, json);
        return file;
    }

    public static List<TestSuite> ParseTestSuitesFromYaml(FileInfo file)
    {
        var dir = file.Directory!;
        List<TestSuite> suites = new();
        var specNode = Yaml.ParseFile<JsonObject>(file);
        Guard.IsNotNull(specNode);
        foreach (var (key, val) in specNode)
        {
            var suiteName = key;
            var suiteJson = val!.AsObject();
            var suite = ParseTestSuite(dir, suiteName, suiteJson);
            suites.Add(suite);
        }

        var spec = new TestSpec { FileName = file.Name, TestSuites = suites };
        return spec;
    }

    public static TestSuite ParseTestSuite(DirectoryInfo dir, string name, JsonObject json)
    {
        var strict = json["strict"]?.GetValue<bool>();
        var options = json["options"]?.AsObject();
        var solvers =
            json["solvers"]?.AsArray().Select(x => x!.GetValue<string>()).ToList()
            ?? new List<string>();
        var includes = json["includes"]!.AsArray().Select(x => x!.GetValue<string>()).ToList();
        var cases = new List<TestCase>();
        var suite = new TestSuite
        {
            Name = name,
            Strict = strict,
            Options = options,
            IncludeGlobs = includes,
            Solvers = solvers,
            TestCases = cases
        };

        foreach (var glob in suite.IncludeGlobs)
        {
            foreach (var file in dir.EnumerateFiles(glob, SearchOption.AllDirectories))
            {
                var path = Path.GetRelativePath(dir.FullName, file.FullName);
                foreach (var testCase in ParseTestCases(file, path))
                    cases.Add(testCase);
            }
        }

        return suite;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    public static IEnumerable<JsonObject> ParseEmbeddedTestCaseJson(FileInfo file)
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
            var doc = Yaml.ParseString<JsonObject>(s);
            if (doc is not null)
                yield return doc;
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

    public static IEnumerable<TestCase> ParseTestCases(FileInfo file, string relativePath)
    {
        foreach (var json in ParseEmbeddedTestCaseJson(file))
        {
            var testName = Path.GetFileNameWithoutExtension(file.Name);
            var solvers = json["solvers"]?.ToNonEmptyList<string>();
            var solveOptions = json["options"]?.AsObject();
            var type = json["type"]?.GetValue<string>();
            var results = new List<TestResult>();
            var extraFiles = json["extra_files"]?.ToNonEmptyList<string>();
            TestResult result;
            if (json["expected"] is JsonArray array)
            {
                if (type is Yaml.COMPILE)
                {
                    var files = array.ToList(x => x.GetString(Yaml.TAG_FLATZINC)!);
                    result = new TestResult { Files = files, Type = ResultType.FlatZinc };
                    results.Add(result);
                }
                else
                {
                    foreach (var resultJson in array)
                    {
                        result = ParseTestResult(resultJson!);
                        results.Add(result);
                    }
                }
            }

            var testCase = new TestCase
            {
                Name = testName,
                Path = relativePath,
                Solvers = solvers,
                SolveOptions = solveOptions,
                Results = results,
                ExtraFiles = extraFiles
            };

            yield return testCase;
        }
    }

    public static TestResult ParseTestResult(JsonNode node)
    {
        var solution = node["solution"];
        var status = node.GetString("status");
        string? tag = null;
        if (node[Yaml.TAG]?.GetValue<string>() is { } t)
        {
            tag = t;
            ((JsonObject)node).Remove(tag);
        }

        TestResult result;
        switch (tag, status, solution)
        {
            case (Yaml.TAG_ERROR, _, null):
                result = new TestResult { Type = ResultType.Error };
                break;
            case (Yaml.TAG_RESULT, Yaml.SATISFIED, _):
                result = new TestResult { Type = ResultType.Satisfied };
                break;
            case (Yaml.TAG_RESULT, _, _):
                result = new TestResult { Type = ResultType.Solution, Solution = solution };
                break;
            default:
                throw new Exception();
        }

        return result;
    }

    private static JsonSerializerOptions? _jsonSerializerOptions;
    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            if (_jsonSerializerOptions is not null)
                return _jsonSerializerOptions;

            var options = new JsonSerializerOptions();
            // options.WriteIndented = true;
            options.WriteIndented = false;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            var converter = new JsonStringEnumConverter();
            options.Converters.Add(converter);
            _jsonSerializerOptions = options;
            return options;
        }
    }
}
