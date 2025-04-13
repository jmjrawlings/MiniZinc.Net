namespace MiniZinc.Tests;

using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using static Console;

enum YamlTag
{
    Set = 1,
    Range,
    Test,
    SingleQuoted,
    DoubleQuoted,
    TestSuite,
    Solution,
    SolutionSet,
    Result,
    EnumConstructor,
    Duration,
    Error,
    Approx,
    FlatZinc,
    OutputModel,
    AnonEnum
}

public sealed class TestParser : IYamlTypeConverter
{
    const string TAG = "__tag__";
    const string VAL = "__val__";
    private IParser _parser = null!;
    public readonly FileInfo? SpecFile;

    public TestParser(FileInfo? file)
    {
        SpecFile = file;
    }

    public bool Accepts(Type type) => true;

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        _parser = parser;
        var node = ParseNode((NodeEvent)_parser.Current!);
        switch (node)
        {
            case JsonObject obj when SpecFile is not null:
                return ParseTestSpec(SpecFile, obj);
            case JsonObject obj:
                return ParseTestCase(obj);
            default:
                return null;
        }
    }

    private TestSpec ParseTestSpec(FileInfo specFile, JsonObject node)
    {
        var testSpec = new TestSpec();
        foreach (var kv in node)
        {
            var name = kv.Key;
            var map = kv.Value!.AsObject();
            ParseTestSuite(specFile, testSpec, name, map);
        }
        return testSpec;
    }

    private void ParseTestSuite(
        FileInfo specFile,
        TestSpec testSpec,
        string suiteName,
        JsonObject suiteNode
    )
    {
        var strict = suiteNode["strict"]?.GetValue<bool>();
        var options = suiteNode["options"]?.AsObject();
        var solvers =
            suiteNode["solvers"]?.AsArray().Select(x => x!.GetValue<string>()).ToList() ?? [];
        var includes = suiteNode["includes"]!.AsArray().Select(x => x!.GetValue<string>()).ToList();
        var testCases = new List<TestCase>();
        var testSuite = new TestSuite
        {
            Name = suiteName,
            Strict = strict,
            Options = options,
            IncludeGlobs = includes,
            Solvers = solvers,
            TestCases = testCases
        };
        var dir = specFile.Directory!;
        foreach (var glob in testSuite.IncludeGlobs)
        {
            foreach (var file in dir.EnumerateFiles(glob, SearchOption.AllDirectories))
            {
                if (file.Extension != ".mzn")
                    continue;

                var relativePath = Path.GetRelativePath(dir.FullName, file.FullName)
                    .Replace('\\', '/');

                foreach (var yaml in ExtractTestCaseYaml(file))
                {
                    var testCase = ParseTestFromString(yaml);
                    if (testCase is null)
                        continue;

                    testCase.Path = relativePath;
                    testCases.Add(testCase);
                }
            }
        }
        testSpec.TestSuites.Add(testSuite);
    }

    private TestCase ParseTestCase(JsonObject node)
    {
        var solvers = node["solvers"]?.ToNonEmptyList<string>();
        var options = (node["solve_options"] ?? node["options"])?.AsObject();
        var allSolutions = options?.Pop("all_solutions").TryGetValue<bool>() ?? false;
        var test_type = node["type"]?.GetValue<string>();
        var inputFiles = node["extra_files"]?.ToNonEmptyList<string>();
        var expected = node["expected"];
        var testCase = new TestCase
        {
            Solvers = solvers,
            Options = options?.Count > 0 ? options : null,
            InputFiles = inputFiles
        };
        testCase.Type = test_type switch
        {
            "compile" => TestType.TEST_COMPILE,
            "output-model" => TestType.TEST_OUTPUT_MODEL,
            _ when allSolutions => TestType.TEST_ALL_SOLUTIONS,
            _ => TestType.TEST_SATISFY
        };
        ParseExpectedSolution(testCase, expected);
        return testCase;
    }

    private JsonNode ParseSet(MappingStart start)
    {
        var sb = new StringBuilder();
        sb.Append('{');
        _parser.MoveNext();
        while (Current is Scalar key)
        {
            _parser.MoveNext();
            ParseNode(Current as NodeEvent);
            sb.Append(key.Value);
            if (Current is not MappingEnd)
                sb.Append(',');
        }

        sb.Append('}');
        var dzn = sb.ToString();
        return dzn;
    }

    private JsonNode? ParseNode(NodeEvent? evt)
    {
        if (evt is null)
            return null;

        var tag = GetTag(evt);
        JsonNode? node = null;

        switch (evt, tag)
        {
            case (MappingStart start, YamlTag.Set):
                node = ParseSet(start);
                break;
            case (MappingStart start, _):
                node = ParseMapping(start, tag);
                break;
            case (SequenceStart start, _):
                node = ParseSequence(start);
                break;
            case (Scalar scalar, null):
                node = ParseScalar(scalar);
                break;
            case (Scalar scalar, YamlTag.Range):
                node = scalar.Value;
                break;
            case (Scalar scalar, _):
                node = new JsonObject();
                node[TAG] = tag.ToString();
                node[VAL] = scalar.Value;
                break;
        }
        _parser.MoveNext();
        return node;
    }

    private JsonNode? ParseScalar(Scalar scalar)
    {
        var value = scalar.Value;
        var style = scalar.Style;
        if (value is "null")
            return null;
        else if (value is "true")
            return true;
        else if (value is "false")
            return false;
        else if (decimal.TryParse(value, out var d))
            return d;
        else if (int.TryParse(value, out var i))
            return i;
        else
            return value;
    }

    private YamlTag? GetTag(NodeEvent e)
    {
        if (e.Tag.IsEmpty)
            return null;

        switch (e.Tag.Value)
        {
            case "tag:yaml.org,2002:set":
            case "!set":
            case "!!set":
                return YamlTag.Set;
            case "!Range":
                return YamlTag.Range;
            case "!Solution":
            case "!Solution:":
                return YamlTag.Solution;
            case "!SolutionSet":
                return YamlTag.SolutionSet;
            case "!Result":
                return YamlTag.Result;
            case "!Test":
                return YamlTag.TestSuite;
            case "!Suite":
                return YamlTag.TestSuite;
            case "!Error":
                return YamlTag.Error;
            case "!ConstrEnum":
            case "tag:yaml.org,2002:python/object:minizinc.types.ConstrEnum":
                return YamlTag.EnumConstructor;
            case "!AnonEnum":
                return YamlTag.AnonEnum;
            case "!FlatZinc":
            case "!FlatZincJSON":
                return YamlTag.FlatZinc;
            case "!OutputModel":
                return YamlTag.OutputModel;
            case "!Approx":
                return YamlTag.Approx;
            case var other:
                WriteLine(other);
                return null;
        }
    }

    private JsonObject ParseMapping(MappingStart start, YamlTag? tag)
    {
        var map = new JsonObject();
        _parser.MoveNext();
        if (tag is not null)
            map[TAG] = tag.ToString();

        while (Current is Scalar key)
        {
            _parser.MoveNext();
            var value = ParseNode(Current as NodeEvent);
            if (map.ContainsKey(key.Value))
                WriteLine($"Duplicate Yaml map key \"{key}\"");
            else
                map.Add(key.Value, value);
        }

        return map;
    }

    private JsonNode ParseSequence(SequenceStart start)
    {
        var tag = GetTag(start);
        var node = new JsonArray();
        _parser.MoveNext();
        while (_parser.Current is not SequenceEnd)
        {
            var item = ParseNode(Current as NodeEvent);
            node.Add(item);
        }
        return node;
    }

    private ParsingEvent Current => _parser.Current!;

    public void WriteYaml(IEmitter emitter, object? value, Type type) { }

    public static TestSpec ParseTestSpecFile(FileInfo file)
    {
        var parser = new TestParser(file);
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTypeConverter(parser)
            .Build();
        var text = File.ReadAllText(file.FullName);
        var spec = deserializer.Deserialize<TestSpec>(text);
        return spec;
    }

    private static TestCase? ParseTestFromString(string yaml)
    {
        var parser = new TestParser(null);
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTypeConverter(parser)
            .Build();
        var testCase = deserializer.Deserialize<TestCase?>(yaml);
        return testCase;
    }

    private void ParseExpectedSolution(TestCase testCase, JsonNode? expected)
    {
        string? dzn = null;
        var tag = GetTag(expected);
        switch (expected, tag)
        {
            case (JsonObject result, YamlTag.Result):
                var solNode = result.Pop("solution");
                var solTag = GetTag(solNode);
                var status = result.TryGetValue<string>("status");
                switch (solTag, solNode, status)
                {
                    case (_, _, "UNSATISFIABLE"):
                        testCase.Type = TestType.TEST_UNSATISFIABLE;
                        break;

                    case (_, JsonArray array, "ALL_SOLUTIONS"):
                        testCase.Type = TestType.TEST_ALL_SOLUTIONS;
                        testCase.Solutions ??= [];
                        foreach (var sol in array)
                        {
                            dzn = ParseSolution(sol?.AsObject());
                            testCase.Solutions ??= [];
                            testCase.Solutions.Add(dzn);
                        }
                        break;

                    case (YamlTag.Solution, JsonObject sol, "OPTIMAL_SOLUTION"):
                        dzn = ParseSolution(sol);
                        testCase.Type = TestType.TEST_OPTIMISE;
                        if (dzn is not null)
                        {
                            testCase.Solutions ??= [];
                            testCase.Solutions.Add(dzn);
                        }
                        break;

                    case (YamlTag.Solution, JsonObject sol, _):
                        dzn = ParseSolution(sol);
                        testCase.Type = TestType.TEST_SATISFY;
                        if (dzn is not null)
                        {
                            testCase.Solutions ??= [];
                            testCase.Solutions.Add(dzn);
                        }
                        break;

                    case (YamlTag.SolutionSet, JsonArray set, _):
                        foreach (var sol in set)
                        {
                            dzn = ParseSolution(sol.AsObject());
                            if (dzn is not null)
                            {
                                testCase.Solutions ??= [];
                                testCase.Solutions.Add(dzn);
                            }
                        }
                        break;
                }
                break;

            case (_, YamlTag.FlatZinc):
                testCase.Type = TestType.TEST_COMPILE;
                testCase.OutputFiles ??= [];
                testCase.OutputFiles.Add(expected[VAL]!.ToString());
                break;

            case (_, YamlTag.OutputModel):
                testCase.Type = TestType.TEST_OUTPUT_MODEL;
                testCase.OutputFiles ??= [];
                testCase.OutputFiles.Add(expected[VAL]!.ToString());
                break;

            case (JsonObject error, YamlTag.Error):
                testCase.ErrorRegex = expected.TryGetValue<string>("regex");
                testCase.ErrorMessage = expected.TryGetValue<string>("message");
                testCase.Type = expected.TryGetValue<string>("type") switch
                {
                    "AssertionError" => TestType.TEST_ASSERTION_ERROR,
                    "EvaluationError" => TestType.TEST_EVALULATION_ERROR,
                    "MiniZincError" => TestType.TEST_MINZINC_ERROR,
                    "TypeError" => TestType.TEST_TYPE_ERROR,
                    "SyntaxError" => TestType.TEST_SYNTAX_ERROR,
                    _ => TestType.TEST_ERROR
                };
                break;
            case (JsonArray results, _):
                foreach (var result in results)
                    ParseExpectedSolution(testCase, result);
                break;
            default:
                throw new Exception();
        }
    }

    private static string? ParseSolution(JsonObject? solution)
    {
        if (solution is not { Count: > 0 })
            return null;

        var tag = GetTag(solution);
        string dzn;
        var sb = new StringBuilder();
        foreach (var (name, node) in solution)
        {
            sb.Append(name);
            sb.Append('=');
            ParseVariable(node, sb);
            sb.Append(';');
        }
        dzn = sb.ToString();
        return dzn;
    }

    private static void ParseVariable(JsonNode? node, StringBuilder sb, bool nested = false)
    {
        var tag = GetTag(node);
        switch (node)
        {
            case null:
                sb.Append("<>");
                break;
            case JsonObject record when tag is null:
                ParseRecord(record, sb);
                break;
            case JsonObject obj when tag is YamlTag.EnumConstructor:
                ParseEnumConstructor(obj, sb);
                break;
            case JsonObject obj when tag is YamlTag.AnonEnum:
                ParseAnonEnum(obj, sb);
                break;
            case JsonObject obj when tag is YamlTag.Approx:
                sb.Append(obj[VAL]);
                break;
            case JsonArray array:
                ParseVariableArray(array, sb, nested);
                break;
            case JsonValue val when val.GetValueKind() is JsonValueKind.String:
                sb.Append(val);
                break;
            default:
                sb.Append(node);
                break;
        }
    }

    private static void ParseAnonEnum(JsonObject obj, StringBuilder sb)
    {
        var name = obj["enumName"];
        var value = obj["value"];
        var dzn = $"{name}({value})";
        sb.Append(dzn);
    }

    private static void ParseEnumConstructor(JsonObject map, StringBuilder sb)
    {
        var name = map["constructor"]!.ToString();
        var arg = map["argument"];
        sb.Append(name);
        sb.Append('(');
        ParseVariable(arg, sb);
        sb.Append(')');
    }

    private static void ParseRecord(JsonObject record, StringBuilder sb)
    {
        sb.Append('(');
        foreach (var (name, field) in record)
        {
            sb.Append(name);
            sb.Append(':');
            ParseVariable(field, sb);
            sb.Append(',');
        }
        sb.Append(')');
    }

    private static void ParseVariableArray(JsonArray array, StringBuilder sb, bool nested)
    {
        if (!nested)
            sb.Append('[');

        int n = array.Count - 1;
        for (int i = 0; i <= n; i++)
        {
            var item = array[i];
            ParseVariable(item, sb, true);
            if (i < n)
                sb.Append(',');
        }

        if (!nested)
            sb.Append(']');
    }

    /// Parse yaml contained within the test file comments
    public static IEnumerable<string> ExtractTestCaseYaml(FileInfo file)
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

    private static YamlTag? GetTag(JsonNode? node)
    {
        if (node is not JsonObject o)
            return null;

        if (!o.ContainsKey(TAG))
            return null;

        var tagString = o[TAG]!.GetValue<string>();
        o.Remove(TAG);
        if (!Enum.TryParse(tagString, out YamlTag tag))
            return null;
        return tag;
    }
}
