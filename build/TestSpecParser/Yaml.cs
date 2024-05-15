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
        public bool Accepts(Type type) => true;

        public object? ReadYaml(IParser parser, Type type)
        {
            if (parser.Current is null)
                return null;
            var result = ParseNode(parser);
            return result;
        }

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

        private JsonNode ParseNode(IParser parser)
        {
            var curr = parser.Current;
            var tag = curr switch
            {
                MappingStart x => GetTag(x),
                SequenceStart x => GetTag(x),
                Scalar x => GetTag(x),
                _ => null
            };

            var node = curr switch
            {
                MappingStart x => ParseMapping(parser, tag),
                SequenceStart x => ParseSequence(parser, tag),
                Scalar x => ParseScalar(parser, tag),
                _ => throw new Exception($"Unexpected event {curr}")
            };
            parser.MoveNext();
            return node;
        }

        private static JsonValue? ParseValue(string str)
        {
            if (str is NULL)
                return null;
            if (str is TRUE)
                return JsonValue.Create(true);
            if (str is FALSE)
                return JsonValue.Create(false);
            if (double.TryParse(str, out var d))
                return JsonValue.Create(d);
            if (int.TryParse(str, out var i))
                return JsonValue.Create(i);
            return JsonValue.Create(str);
        }

        private static JsonNode ParseScalar(IParser parser, string? tag)
        {
            var str = (parser.Current as Scalar)!.Value;
            JsonNode? node = ParseValue(str);

            if (tag is null)
                return node!;

            var type = GetTypeNameFromTag(tag);
            if (tag is TAG_RANGE)
            {
                var tokens = str.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var lower = ParseValue(tokens[0]);
                var upper = ParseValue(tokens[1]);
                var arr = new JsonArray(lower, upper);
                var obj = new JsonObject { { RANGE, arr } };
                return obj;
            }

            // Empty string with a tag should just be an object
            if (string.IsNullOrEmpty(str))
                node = new JsonObject { { TAG, tag } };
            else
                node = new JsonObject { { type, node } };

            return node;
        }

        private static JsonObject ParseSet(IParser parser)
        {
            JsonArray arr = new();
            parser.MoveNext();
            loop:
            var curr = parser.Current;
            switch (curr)
            {
                case null:
                case MappingEnd:
                    break;
                default:
                    var key = (parser.Current as Scalar)!.Value;
                    var value = ParseValue(key);
                    arr.Add(value);
                    parser.MoveNext();
                    parser.MoveNext();
                    goto loop;
            }

            var obj = new JsonObject { { SET, arr } };
            return obj;
        }

        private JsonNode ParseMapping(IParser parser, string? tag)
        {
            if (tag is TAG_SET)
            {
                var set = ParseSet(parser);
                return set;
            }

            JsonObject map = new();
            parser.MoveNext();
            loop:
            var curr = parser.Current;
            switch (curr)
            {
                case null:
                case MappingEnd:
                    break;
                default:
                    // Keys are always scalars
                    var key = (parser.Current as Scalar)!.Value;
                    parser.MoveNext();
                    var value = ParseNode(parser);
                    if (map.ContainsKey(key))
                        WriteLine($"Duplicate Yaml map key \"{key}\"");
                    else
                        map.Add(key, value);
                    goto loop;
            }

            if (tag is not null)
                map[TAG] = tag;

            return map;
        }

        private JsonNode ParseSequence(IParser parser, string? _tag)
        {
            parser.MoveNext();
            var node = new JsonArray();

            loop:
            var curr = parser.Current;
            switch (curr)
            {
                case null:
                    break;
                case SequenceEnd:
                    break;
                default:
                    var item = ParseNode(parser);
                    node.Add(item);
                    goto loop;
            }
            return node;
        }
    }

    public static JsonNode ParseString(string s)
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

    public static T ParseString<T>(string s)
        where T : JsonNode
    {
        var result = ParseString(s);

        if (result is T t)
            return t;

        throw new Exception($"Yaml string was parsed as a {result} but expected a {typeof(T)}");
    }

    public static JsonNode ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }

    public static T ParseFile<T>(FileInfo fi)
        where T : JsonNode
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString<T>(text);
        return node;
    }
}
