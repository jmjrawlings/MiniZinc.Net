namespace MiniZinc.Net.Tests;

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
    public const string VALUE = "value";
    public const string LOWER = "lower";
    public const string UPPER = "upper";
    public const string OUTPUT_MODEL = "outputmodel";
    public const string TYPE = "type";
    public const string SET = "set";
    public const string RANGE = "range";
    public const string DURATION = "duration";
    public const string SOLUTION = "solution";
    public const string SATISFIED = "SATISFIED";

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
            var result = Parse(parser);
            return result;
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type) { }

        private static string? GetTag(NodeEvent e)
        {
            if (e.Tag.IsEmpty)
                return null;
            return e.Tag.Value;
        }

        private static string GetTypeFromTag(string tag)
        {
            var c = tag.Where(char.IsLetter).Select(char.ToLower).ToArray();
            var s = new string(c);
            return s;
        }

        private JsonNode Parse(IParser parser)
        {
            var curr = parser.Current;
            // Console.WriteLine("{0} {1} {2}", curr.Start, curr.End, curr.GetType().Name);
            var node = curr switch
            {
                MappingStart x => Parse(parser, x),
                SequenceStart x => ParseSequence(parser, x),
                Scalar x => ParseScalar(x),
                _ => throw new Exception($"Unexpected yaml event {curr}")
            };
            parser.MoveNext();
            return node;
        }

        private JsonNode ParseScalar(Scalar scalar)
        {
            var tag = GetTag(scalar);
            var str = scalar.Value;
            var node = ParseScalar(str, tag);
            return node;
        }

        private JsonNode ParseScalar(string str, string? tag = null)
        {
            JsonNode node;
            if (str is TRUE)
                node = JsonValue.Create(true);
            else if (str is FALSE)
                node = JsonValue.Create(false);
            else if (double.TryParse(str, out var d))
                node = JsonValue.Create(d);
            else if (int.TryParse(str, out var i))
                node = JsonValue.Create(i);
            else
                node = JsonValue.Create(str);

            if (tag is null)
                return node;

            var type = GetTypeFromTag(tag);
            if (tag is TAG_RANGE)
            {
                var tokens = str.Split('.', StringSplitOptions.RemoveEmptyEntries);
                var lower = ParseScalar(tokens[0]);
                var upper = ParseScalar(tokens[1]);
                var obj = new JsonObject
                {
                    { LOWER, lower },
                    { UPPER, upper },
                    { TYPE, type }
                };
                return obj;
            }

            // Empty string with a tag should just be an object
            if (string.IsNullOrEmpty(str))
                node = new JsonObject { { TAG, tag } };
            else
                node = new JsonObject { { type, node } };

            return node;
        }

        private JsonNode Parse(IParser parser, MappingStart e)
        {
            JsonObject map = new();
            parser.MoveNext();
            loop:
            var curr = parser.Current;
            var tag = GetTag(e);
            switch (curr)
            {
                case null:
                    break;
                case MappingEnd:
                    break;
                default:
                    var key = (parser.Current as Scalar)!.Value;
                    parser.MoveNext();
                    var value = Parse(parser);
                    if (map.ContainsKey(key))
                        WriteLine($"Duplicate Yaml map key \"{key}\"");
                    else
                        map.Add(key, value);
                    goto loop;
            }

            if (tag is TAG_SET)
            {
                var set = new JsonArray();
                foreach (var (key, _) in map)
                {
                    var key_ = ParseScalar(key);
                    set.Add(key_);
                }

                var node = new JsonObject { { TAG, TAG_SET }, { SET, set } };
                return node;
            }

            if (tag is not null)
                map[TAG] = tag;

            return map;
        }

        private JsonNode ParseSequence(IParser parser, SequenceStart e)
        {
            parser.MoveNext();
            var node = new JsonArray();

            loop:
            var curr = parser.Current;
            var tag = GetTag(e);
            switch (curr)
            {
                case null:
                    break;
                case SequenceEnd:
                    break;
                default:
                    var item = Parse(parser);
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
        if (result is null)
            return null;

        if (result is T t)
            return t;

        throw new Exception($"Yaml string was parsed as a {result} but expected a {typeof(T)}");
    }

    public static JsonNode? ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }

    public static T? ParseFile<T>(FileInfo fi)
        where T : JsonNode
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString<T>(text);
        return node;
    }
}
