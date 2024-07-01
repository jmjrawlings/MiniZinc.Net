namespace LibMiniZinc.Tests;

using System.Text;
using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using static Console;

/// <summary>
/// Handles the parsing of test case definitions stored as
/// Yaml literals embedded in the comments of the libminizinc
/// test files.
/// </summary>
/// <remarks>
/// This is extremely specific to the tags and structure
/// used by libminizinc and not intended for general Yaml
/// parsing
/// </remarks>
public static class Yaml
{
    public const string TAG_RANGE = "!Range";
    public const string TAG_SET = "tag:yaml.org,2002:set";
    public const string TAG_APPROX = "!Approx";
    public const string TAG_TEST = "!Test";
    public const string TAG_RESULT = "!Result";
    public const string TAG_SOLUTION_SET = "!SolutionSet";
    public const string TAG_SOLUTION = "!Solution";
    public const string TAG_DURATION = "!Duration";
    public const string TAG_ERROR = "!Error";
    public const string TAG_FLATZINC = "!FlatZinc";
    public const string FLATZINC = "flatzinc";
    public const string OPTIMAL = "OPTIMAL_SOLUTION";
    public const string TAG = "__tag__";
    public const string TRUE = "true";
    public const string FALSE = "false";
    public const string NULL = "null";
    public const string OUTPUT_MODEL = "outputmodel";
    public const string SET = "set";
    public const string RANGE = "range";
    public const string DURATION = "duration";
    public const string SOLUTION = "solution";
    public const string SATISFIED = "SATISFIED";
    public const string UNSATISFIABLE = "UNSATISFIABLE";

    /*
     Used to parse the Yaml contained in:
     1. The 'spec.yaml' test suite file
     2. The embedded yaml in each `.mzn` test file
     */
    private sealed class Converter : IYamlTypeConverter
    {
        private IParser _parser = null!;

        // Are we in a !Solution section?
        private bool _inSolution = false;

        // Are we in a solution variable?
        private bool _inSolutionVariable = false;

        public bool Accepts(Type type) => true;

        public object? ReadYaml(IParser parser, Type type)
        {
            _parser = parser;
            if (Current is null)
                return null;
            var node = ParseNode();
            return node;
        }

        private ParsingEvent? Current => _parser.Current;

        public void WriteYaml(IEmitter emitter, object? value, Type type) { }

        private static string? GetTag(NodeEvent e)
        {
            if (e.Tag.IsEmpty)
                return null;
            return e.Tag.Value;
        }

        private static string GetTypeNameFromTag(string tag)
        {
            var c = tag.Where(char.IsLetter).Select(char.ToLower).ToArray();
            var s = new string(c);
            return s;
        }

        private JsonNode ParseNode()
        {
            var tag = Current switch
            {
                MappingStart x => GetTag(x),
                SequenceStart x => GetTag(x),
                Scalar x => GetTag(x),
                _ => null
            };

            if (tag is TAG_SOLUTION)
                _inSolution = true;

            var node = Current switch
            {
                MappingStart start => ParseMapping(start, tag),
                SequenceStart start => ParseSequence(start, tag),
                Scalar scalar => ParseScalar(scalar, tag),
                var x => throw new Exception($"Unexpected event {x}")
            };
            _parser.MoveNext();
            _inSolution = false;
            return node;
        }

        private JsonValue? ParseValue(Scalar scalar)
        {
            var value = scalar.Value;
            var style = scalar.Style;
            if (value is NULL)
                return null;
            if (value is TRUE)
                return JsonValue.Create(true);
            if (value is FALSE)
                return JsonValue.Create(false);
            if (decimal.TryParse(value, out var d))
                return JsonValue.Create(d);
            if (int.TryParse(value, out var i))
                return JsonValue.Create(i);
            if (style is ScalarStyle.DoubleQuoted or ScalarStyle.SingleQuoted)
                if (_inSolution)
                    return JsonValue.Create($"\"{value}\"");

            return JsonValue.Create(value);
        }

        private JsonNode ParseScalar(Scalar scalar, string? tag)
        {
            JsonNode? node = ParseValue(scalar);
            if (tag is null or TAG_APPROX)
                return node!;

            var type = GetTypeNameFromTag(tag);
            // Empty string with a tag should just be an object
            if (string.IsNullOrEmpty(scalar.Value))
                node = new JsonObject { { TAG, tag } };
            else
                node = new JsonObject { { type, node } };

            return node;
        }

        private JsonValue ParseSet()
        {
            var sb = new StringBuilder();
            sb.Append('{');
            _parser.MoveNext();
            int i = 0;
            while (Current is Scalar scalar)
            {
                if (i++ > 1)
                    sb.Append(',');

                var value = ParseValue(scalar);
                sb.Append(value);
                _parser.MoveNext();
                _parser.MoveNext();
            }
            sb.Append('}');
            var dzn = sb.ToString();
            var set = JsonValue.Create(dzn);
            return set;
        }

        private JsonNode ParseMapping(MappingStart start, string? tag)
        {
            if (tag is TAG_SET)
            {
                var set = ParseSet();
                return set;
            }

            JsonObject map = new();
            _parser.MoveNext();
            while (Current is Scalar s)
            {
                var key = s.Value;
                _parser.MoveNext();
                if (_inSolution && !_inSolutionVariable)
                    _inSolutionVariable = true;

                var value = ParseNode();
                if (map.ContainsKey(key))
                    WriteLine($"Duplicate Yaml map key \"{key}\"");
                else
                    map.Add(key, value);
                _inSolutionVariable = false;
            }

            if (tag is not null)
                map[TAG] = tag;

            return map;
        }

        private JsonNode ParseSequence(SequenceStart start, string? _tag)
        {
            _parser.MoveNext();
            var node = new JsonArray();

            loop:
            switch (Current)
            {
                case null:
                    break;
                case SequenceEnd:
                    break;
                default:
                    var item = ParseNode();
                    node.Add(item);
                    goto loop;
            }
            return node;
        }
    }

    public static JsonNode? ParseString(string s)
    {
        var deserializer = new DeserializerBuilder()
            .WithTagMapping(TAG_TEST, typeof(object))
            .WithTagMapping(TAG_RESULT, typeof(object))
            .WithTagMapping(TAG_SOLUTION_SET, typeof(object))
            .WithTagMapping(TAG_SOLUTION, typeof(object))
            .WithTagMapping(TAG_DURATION, typeof(object))
            .WithTagMapping(TAG_ERROR, typeof(object))
            .WithTagMapping(TAG_FLATZINC, typeof(object))
            .WithTagMapping(TAG_RANGE, typeof(object))
            .WithTagMapping(TAG_SET, typeof(object))
            .WithTypeConverter(new Converter())
            .Build();
        var text = s.TrimEnd();
        var node = deserializer.Deserialize<JsonNode>(text);
        return node;
    }

    public static T? ParseString<T>(string s)
        where T : JsonNode
    {
        var result = ParseString(s);

        if (result is T t)
            return t;

        return null;
    }

    public static T ParseFile<T>(FileInfo fi)
        where T : JsonNode
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString<T>(text);
        return node;
    }
}
