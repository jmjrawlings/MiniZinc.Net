namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

/*
 Used to parse the Yaml contained in:
 1. The 'spec.yaml' test suite file
 2. The embedded yaml in each `.mzn` test file
 */
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
        return e.Tag.Value;
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
                var value = ParseNode(parser);
                map.Add(key, value);
                goto loop;
        }

        // Store the tag as a property
        if (tag is not null)
            map["__tag__"] = tag;

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
