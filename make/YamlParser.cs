namespace MiniZinc.Tests;

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Command;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using static Console;

public sealed class YamlParser : IYamlTypeConverter
{
    const string TAG = "__tag__";
    const string VAL = "__val__";
    private IParser _parser = null!;
    public readonly FileInfo? SpecFile;

    public YamlParser(FileInfo? file)
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
                return ParseTestSpecFromFile(SpecFile, obj);
            case JsonObject obj:
                return ParseTestCaseFromYaml(obj);
            default:
                return null;
        }
    }

    private TestSpec ParseTestSpecFromFile(FileInfo specFile, JsonObject node)
    {
        var spec = new TestSpec();
        foreach (var kv in node)
        {
            var suiteName = kv.Key;
            var suiteJson = kv.Value!.AsObject();
            ParseTestSuite(specFile, spec, suiteName, suiteJson);
        }
        return spec;
    }

    private void ParseTestSuite(
        FileInfo testSpecFile,
        TestSpec testSpec,
        string testSuiteName,
        JsonObject testSuiteNode
    )
    {
        // var strict = suiteNode["strict"]?.GetValue<bool>();
        JsonObject? suiteOptions = testSuiteNode["options"]?.AsObject();
        List<string>? suiteSolvers = testSuiteNode["solvers"]
            ?.AsArray()
            .Select(x => x!.GetValue<string>())
            .ToList();
        List<string> suiteGlobs = testSuiteNode["includes"]!
            .AsArray()
            .Select(x => x!.GetValue<string>())
            .ToList();
        DirectoryInfo specDir = testSpecFile.Directory!;

        foreach (string suiteGlob in suiteGlobs)
        {
            foreach (
                FileInfo testFile in specDir.EnumerateFiles(suiteGlob, SearchOption.AllDirectories)
            )
            {
                if (testFile.Extension != ".mzn")
                    continue;

                string testPath = Path.GetRelativePath(specDir.FullName, testFile.FullName)
                    .Replace('\\', '/');

                foreach (string testCasesYaml in GetTestCaseYaml(testFile))
                {
                    TestCase? testCase = ParseTestCaseFromYaml(testCasesYaml);
                    if (testCase is null)
                        continue;

                    testCase.Path = testPath;
                    testCase.Suite = testSuiteName;
                    var opts = testCase.Options;
                    var solvers = (testCase.Solvers ?? suiteSolvers ?? []).ToImmutableArray();

                    if (suiteOptions is not null)
                    {
                        foreach (var kv in suiteOptions)
                        {
                            if (opts?.ContainsKey(kv.Key) ?? false)
                                continue;

                            opts ??= new();
                            opts[kv.Key] = kv.Value.DeepClone();
                        }
                    }
                    ParseOptions(ref opts, out bool? allSolutions, out var args);
                    testCase.Options = opts;
                    testCase.Args = args?.ToString();
                    if (allSolutions is true)
                        testCase.Type = TestType.ALL_SOLUTIONS;

                    testSpec.TestCases ??= [];

                    if (solvers.Length > 0)
                    {
                        testCase.Solvers = [];
                        testCase.Solvers.AddRange(solvers);
                    }

                    if (testCase.Type is TestType.SOLVE && testCase.Solutions is { Count: > 1 })
                        testCase.Type = TestType.ANY_SOLUTION;

                    testSpec.TestCases.Add(testCase);
                }
            }
        }
    }

    private void ParseOptions(ref JsonObject? node, out bool? allSolutions, out Args? args)
    {
        /*
            time_limit: Optional[timedelta] = None,
            nr_solutions: Optional[int] = None,
            processes: Optional[int] = None,
            random_seed: Optional[int] = None,
            all_solutions: bool = False,
            intermediate_solutions: Optional[bool] = None,
            free_search: bool = False,
            optimisation_level: Optional[int] = None,
            timeout: Optional[timedelta] = None
         */
        allSolutions = null;
        args = null;
        if (node is null)
            return;

        var map = node.AsObject();
        if (map.Count is 0)
        {
            node = null;
            return;
        }

        args = Args.Empty;
        var items = map.ToArray();
        foreach (var kv in items)
        {
            var flag = kv.Key;
            var value = kv.Value;
            var kind = value?.GetValueKind() ?? default;
            switch (flag)
            {
                case "all_solutions":
                    allSolutions = value!.GetValue<bool>();
                    map.Remove(flag);
                    break;
                case "time_limit":
                    args.Add($"--time-limit {value}");
                    map.Remove(flag);
                    break;
                case var x when x.StartsWith('-') && kind is JsonValueKind.True:
                    args.Add($"{flag}");
                    map.Remove(flag);
                    break;
                case var x when kind is JsonValueKind.True:
                    args.Add($"--{flag}");
                    map.Remove(flag);
                    break;
                default:
                    if (flag.StartsWith('-'))
                        args.Add($"{flag} {value}");
                    else
                        args.Add($"--{flag} {value}");

                    map.Remove(flag);
                    break;
            }
        }

        if (map.Count is 0)
            node = null;

        if (args.Count is 0)
            args = null;
    }

    private TestCase ParseTestCaseFromYaml(JsonObject node)
    {
        var solvers = node["solvers"]?.ToNonEmptyList<string>();
        var test_type = node["type"]?.GetValue<string>();
        var inputFiles = node["extra_files"]?.ToNonEmptyList<string>();
        var expected = node["expected"];
        var options = node["options"]?.AsObject();
        var testCase = new TestCase { ExtraFiles = inputFiles, Options = options, };
        var against = node["check_against"]?.ToNonEmptyList<string>();
        testCase.Solvers = solvers;
        testCase.Type = test_type switch
        {
            "compile" => TestType.COMPILE,
            "output-model" => TestType.OUTPUT_MODEL,
            _ => TestType.SOLVE,
        };
        ParseExpectedSolution(testCase, expected, against);
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
        {
            return value;
        }
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
        var parser = new YamlParser(file);
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTypeConverter(parser)
            .Build();
        var text = File.ReadAllText(file.FullName);
        var spec = deserializer.Deserialize<TestSpec>(text);
        return spec;
    }

    public static TestCase? ParseTestCaseFromYaml(string yaml)
    {
        var parser = new YamlParser(null);
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTypeConverter(parser)
            .Build();
        var testCase = deserializer.Deserialize<TestCase?>(yaml);
        return testCase;
    }

    private void ParseExpectedSolution(
        TestCase testCase,
        JsonNode? expected,
        List<string>? checkAgainst
    )
    {
        string? dzn = null;
        bool output = false;
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
                        testCase.Type = TestType.UNSATISFIABLE;
                        break;

                    case (_, JsonArray array, "ALL_SOLUTIONS"):
                        testCase.Type = TestType.ALL_SOLUTIONS;
                        testCase.Solutions ??= [];
                        foreach (var sol in array)
                        {
                            (dzn, output) = ParseSolution(sol?.AsObject());
                            testCase.Solutions ??= [];
                            testCase.Solutions.Add(dzn);
                        }
                        break;

                    case (_, _, _) when checkAgainst is not null:
                        testCase.Type = TestType.CHECK_AGAINST;
                        testCase.CheckAgainstSolvers = checkAgainst;
                        break;

                    case (YamlTag.Solution, JsonObject sol, _):
                        (dzn, output) = ParseSolution(sol);

                        if (output)
                            testCase.Type = TestType.OUTPUT;

                        testCase.Solutions ??= [];
                        testCase.Solutions.Add(dzn);
                        break;

                    case (YamlTag.SolutionSet, JsonArray set, _):
                        foreach (var sol in set)
                        {
                            (dzn, output) = ParseSolution(sol.AsObject());
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
                testCase.Type = TestType.COMPILE;
                testCase.OutputFiles ??= [];
                testCase.OutputFiles.Add(expected[VAL]!.ToString());
                break;

            case (_, YamlTag.OutputModel):
                testCase.Type = TestType.OUTPUT_MODEL;
                testCase.OutputFiles ??= [];
                testCase.OutputFiles.Add(expected[VAL]!.ToString());
                break;

            case (JsonObject error, YamlTag.Error):
                testCase.ErrorRegex = expected.TryGetValue<string>("regex");
                testCase.ErrorMessage = expected.TryGetValue<string>("message");
                testCase.Type = expected.TryGetValue<string>("type") switch
                {
                    "AssertionError" => TestType.ASSERTION_ERROR,
                    "EvaluationError" => TestType.EVALUATION_ERROR,
                    "MiniZincError" => TestType.MINIZINC_ERROR,
                    "TypeError" => TestType.TYPE_ERROR,
                    "SyntaxError" => TestType.SYNTAX_ERROR,
                    _ => TestType.ERROR
                };
                break;
            case (JsonArray results, _):
                foreach (var result in results)
                    ParseExpectedSolution(testCase, result, checkAgainst);
                break;
            default:
                break;
                throw new Exception();
        }
    }

    private static (string?, bool) ParseSolution(JsonObject? solution)
    {
        if (solution is not { Count: > 0 })
            return (null, false);

        var tag = GetTag(solution);
        string dzn;
        bool output = false;
        var sb = new StringBuilder();
        foreach (var (name, node) in solution)
        {
            if (name == "_output_item")
            {
                ParseVariable(node, sb);
                output = true;
                break;
            }

            sb.Append(name);
            sb.Append('=');
            ParseVariable(node, sb);
            sb.Append(';');
        }
        dzn = sb.ToString();
        return (dzn, output);
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
                var x = $"\"{val}\"";
                sb.Append(x);
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
    public static IEnumerable<string> GetTestCaseYaml(FileInfo file)
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
