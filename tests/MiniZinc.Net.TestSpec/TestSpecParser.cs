namespace MiniZinc.Net.Tests;

using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using CommunityToolkit.Diagnostics;
using YamlDotNet.Serialization;

internal sealed class TestSpecParser
{
    public readonly FileInfo YamlFile;
    public readonly DirectoryInfo Directory;
    private readonly StringBuilder _sb;
    public readonly TestSpec Spec;

    internal TestSpecParser(FileInfo file)
    {
        YamlFile = file;
        Directory = file.Directory!;
        _sb = new StringBuilder();
        List<TestSuite> suites = new();
        var array = Yaml.ParseFile(file) as JsonArray;
        Guard.IsNotNull(array);
        foreach (var node in array)
        {
            var map = node!.AsObject();
            var suite = ParseTestSuite(map);
            suites.Add(suite);
        }

        Spec = new TestSpec { FileName = file.Name, TestSuites = suites };
    }

    private TestSuite ParseTestSuite(JsonObject node)
    {
        var strict = node["strict"]?.GetValue<bool>();
        var options = node["options"]?.AsObject();
        var solvers = node["solvers"]!.AsArray().Select(x => x!.GetValue<string>()).ToList();
        var includes = node["includes"]!.AsArray().Select(x => x!.GetValue<string>()).ToList();
        var cases = new List<TestCase>();
        var suite = new TestSuite
        {
            Name = node.GetPropertyName(),
            Strict = strict,
            Options = options,
            IncludeGlobs = includes,
            Solvers = solvers,
            TestCases = cases
        };

        foreach (var glob in suite.IncludeGlobs)
        {
            foreach (var file in Directory.EnumerateFiles(glob, SearchOption.AllDirectories))
            {
                var path = Path.GetRelativePath(Directory.FullName, file.FullName);

                foreach (var testCase in ParseTestCases(path))
                {
                    cases.Add(testCase);
                }
            }
        }

        return suite;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    private IEnumerable<JsonNode> ParseTestCase(string relativePath)
    {
        var path = Path.Join(Directory.FullName, relativePath);
        var file = new FileInfo(path);
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);
        _sb.Clear();

        const char EOF = '\uffff';
        char c = EOF;
        int i = -1;
        void Read()
        {
            i++;
            c = (char)reader.Read();
        }

        bool Skip(char c)
        {
            if (reader.Peek() != c)
                return false;

            Read();
            return true;
        }

        if (!Skip('/'))
            yield break;

        preamble:
        Read();
        if (c is '\n' or '*' or '-')
            goto preamble;
        if (char.IsWhiteSpace(c))
            goto preamble;

        contents:
        _sb.Append(c);
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

        var yamlString = _sb.ToString();
        var testStrings = yamlString.Split(
            "---",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        foreach (var s in testStrings)
        {
            var doc = Yaml.ParseString(s);
            if (doc is not null)
                yield return doc;
        }
    }

    private IEnumerable<TestCase> ParseTestCases(string path)
    {
        foreach (var node in ParseTestCase(path))
        {
            var testName = Path.GetFileNameWithoutExtension(path);
            var solvers = node["solvers"]!.AsArray().Select(x => x.GetValue<string>()).ToList();
            var solveOptions = node["options"] ?? new JsonObject();
            var results = new List<TestResult>();
            var testCase = new TestCase
            {
                TestName = testName,
                TestPath = path,
                Solvers = solvers,
                SolveOptions = solveOptions,
                Results = results
            };
            var check = node["check_against"];
            var extra = node["extra"];
            if (node["expected"] is JsonArray array)
            {
                foreach (var expected in array)
                {
                    var result = ParseTestResult(expected);
                    testCase.Results.Add(result);
                }
            }

            yield return testCase;
        }
    }

    private TestResult ParseTestResult(JsonNode? node)
    {
        var obj = node?.AsObject()!;
        var sol = obj["solution"];
        var result = new TestResult { Solution = sol };
        return result;
    }
}
