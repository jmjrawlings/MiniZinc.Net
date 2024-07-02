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
    public enum Tag
    {
        Set = 1,
        Range,
        Test,
        TestSuite,
        Solution,
        Result,
        AnonEnum,
        EnumConstructor,
        Duration,
        Error,
        Approx,
        FlatZinc
    }

    public const string TAG = "__tag__";
    public const string DZN = "__dzn__";

    /*
     Used to parse the Yaml contained in:
     1. The 'spec.yaml' test suite file
     2. The embedded yaml in each `.mzn` test file
     */
    private sealed class Converter : IYamlTypeConverter
    {
        private IParser _parser = null!;

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

        private JsonNode? ParseNode(bool inSolution = false)
        {
            var tag = GetTag((NodeEvent)Current!);
            if (tag is not null)
                WriteLine($"{tag} - {Current.GetType().Name}");

            JsonNode? node = null;

            switch (Current, tag)
            {
                case (MappingStart start, Tag.Set):
                    node = ParseSet(start);
                    break;
                case (MappingStart start, Tag.EnumConstructor):
                    node = ParseEnumConstructor(start);
                    break;
                case (MappingStart start, _):
                    node = ParseMapping(start, tag, inSolution);
                    break;
                case (SequenceStart start, _):
                    node = ParseSequence(start, tag, inSolution);
                    break;
                case (Scalar _, Tag.Solution):
                    node = new JsonObject();
                    break;
                case (Scalar scalar, Tag.Range):
                    node = ParseRange(scalar);
                    break;
                case (Scalar scalar, _):
                    node = ParseScalar(scalar, inSolution);
                    break;
            }
            _parser.MoveNext();
            return node;
        }

        private JsonNode ParseEnumConstructor(MappingStart start)
        {
            var sb = new StringBuilder();
            _parser.MoveNext();
            while (Current is Scalar s)
            {
                var key = s.Value;
                _parser.MoveNext();
                var value = ParseNode(true);
                if (key is "constructor")
                {
                    sb.Append(value);
                    sb.Append('(');
                }
                else if (key is "argument")
                {
                    sb.Append(value);
                    sb.Append(')');
                }
                else
                {
                    throw new Exception();
                }
            }

            var dzn = sb.ToString();
            var map = new JsonObject();
            map[TAG] = Tag.EnumConstructor.ToString();
            map["dzn"] = dzn;
            return map;
        }

        private JsonNode? ParseScalar(Scalar scalar, bool inSolution = false)
        {
            var value = scalar.Value;
            var style = scalar.Style;
            JsonNode? node = null;
            if (value is "null") { }
            else if (value is "true")
                node = JsonValue.Create(true);
            else if (value is "false")
                node = JsonValue.Create(false);
            else if (decimal.TryParse(value, out var d))
                node = JsonValue.Create(d);
            else if (int.TryParse(value, out var i))
                node = JsonValue.Create(i);
            else if (style is ScalarStyle.DoubleQuoted or ScalarStyle.SingleQuoted)
            {
                if (inSolution)
                    node = JsonValue.Create($"\"{value}\"");
                else
                    node = JsonValue.Create(value);
            }
            else
            {
                node = JsonValue.Create(value);
            }
            return node;
        }

        private Tag? GetTag(NodeEvent e)
        {
            if (e.Tag.IsEmpty)
                return null;

            switch (e.Tag.Value)
            {
                case "tag:yaml.org,2002:set":
                case "!set":
                case "!!set":
                    return Tag.Set;
                case "!Range":
                    return Tag.Range;
                case "!Solution":
                case "!Solution:":
                    return Tag.Solution;
                case "!Result":
                    return Tag.Result;
                case "!Test":
                    return Tag.TestSuite;
                case "!Suite":
                    return Tag.TestSuite;
                case "!Error":
                    return Tag.Error;
                case "!ConstrEnum":
                case "tag:yaml.org,2002:python/object:minizinc.types.ConstrEnum":
                    return Tag.EnumConstructor;
                case "!AnonEnum":
                    return Tag.AnonEnum;
                case "!FlatZinc":
                    return Tag.FlatZinc;
                case "!Approx":
                    return Tag.Approx;
                case var other:
                    WriteLine(other);
                    return null;
            }
        }

        private JsonNode ParseMapping(MappingStart start, Tag? tag, bool inSolution = false)
        {
            var map = new JsonObject();
            _parser.MoveNext();
            if (tag is not null)
                map[TAG] = tag.ToString();

            if (tag is Tag.Solution)
                inSolution = true;

            while (Current is Scalar s)
            {
                var key = s.Value;
                _parser.MoveNext();
                var value = ParseNode(inSolution);
                if (map.ContainsKey(key))
                    WriteLine($"Duplicate Yaml map key \"{key}\"");
                else
                    map.Add(key, value);
            }

            return map;
        }

        private JsonNode ParseSet(MappingStart start)
        {
            _parser.MoveNext();
            var map = new JsonObject();
            var sb = new StringBuilder();
            sb.Append('{');
            int i = 0;
            while (Current is Scalar key)
            {
                if (++i > 1)
                    sb.Append(',');
                sb.Append(key.Value);
                _parser.MoveNext();
                _parser.MoveNext();
            }

            sb.Append('}');
            var dzn = sb.ToString();
            map["_set_"] = dzn;
            return map;
        }

        private JsonObject ParseRange(Scalar scalar)
        {
            var tokens = scalar.Value.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var lower = int.Parse(tokens[0]);
            var upper = int.Parse(tokens[1]);
            var sb = new StringBuilder();
            sb.Append('{');

            for (int item = lower; item <= upper; item++)
            {
                if (item > lower)
                    sb.Append(',');
                sb.Append(item);
            }

            sb.Append('}');
            var set = new JsonObject();
            set["_set_"] = sb.ToString();
            return set;
        }

        private JsonNode ParseSequence(SequenceStart start, Tag? tag, bool inSolution = false)
        {
            var node = new JsonArray();
            _parser.MoveNext();
            while (Current is not SequenceEnd)
            {
                var item = ParseNode();
                node.Add(item);
            }
            return node;
        }
    }

    public static JsonNode? ParseString(string s)
    {
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
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
