using System.Text;
using System.Text.Json.Nodes;
using MiniZinc.Net.Tests;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public static class Yaml
{
    public static JsonNode? ParseString(string s)
    {
        var converter = new YamlConverter();
        var deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTagMapping("!Result", typeof(object))
            .WithTagMapping("!SolutionSet", typeof(object))
            .WithTagMapping("!Solution", typeof(object))
            .WithTagMapping("!Duration", typeof(object))
            .WithTypeConverter(converter)
            .Build();
        var text = s.TrimEnd();
        var node = deserializer.Deserialize(text) as JsonNode;
        return node;
    }

    public static JsonNode? ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }
}

internal sealed class YamlConverter : IYamlTypeConverter
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

    internal static string? GetTag(NodeEvent e)
    {
        if (e.Tag.IsEmpty)
            return null;
        var s = e.Tag.Value.Where(char.IsLetter);
        var x = string.Join("", s).ToUpper();
        Console.WriteLine(x);
        return x;
    }

    private JsonNode ParseNode(IParser parser)
    {
        var curr = parser.Current;
        // Console.WriteLine("{0} {1} {2}", curr.Start, curr.End, curr.GetType().Name);
        var node = curr switch
        {
            MappingStart x => ParseMap(parser, x),
            SequenceStart x => ParseList(parser, x),
            Scalar x => ParseValue(parser, x),
            _ => throw new Exception($"Unexpected yaml event {curr}")
        };
        parser.MoveNext();
        return node;
    }

    public const string TRUE = "true";
    public const string FALSE = "false";
    public const string DURATION = "duration";

    private JsonValue ParseValue(IParser parser, Scalar scalar)
    {
        var tag = GetTag(scalar);
        var str = scalar.Value;
        JsonValue val;
        if (tag == DURATION)
        {
            var duration = TimeSpan.Parse(str);
            val = JsonValue.Create(duration.ToString());
        }
        else if (str is TRUE)
            val = JsonValue.Create(true);
        else if (str is FALSE)
            val = JsonValue.Create(false);
        else if (int.TryParse(str, out var i))
            val = JsonValue.Create(i);
        else if (double.TryParse(str, out var d))
            val = JsonValue.Create(d);
        else
            val = JsonValue.Create(str);
        return val;
    }

    private JsonNode ParseMap(IParser parser, MappingStart e)
    {
        var map = new JsonObject();
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
                var value = ParseNode(parser);
                var kvp = KeyValuePair.Create(key, value);
                map.Add(kvp);
                goto loop;
        }
        return map;
    }

    private JsonNode ParseList(IParser parser, SequenceStart e)
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
                var item = ParseNode(parser);
                node.Add(item);
                goto loop;
        }
        return node;
    }
}
