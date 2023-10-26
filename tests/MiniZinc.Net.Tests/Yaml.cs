
using System.Text;

namespace MiniZinc.Tests;

using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using System.Collections;

public sealed class YamlReader : IYamlTypeConverter
{
    public bool Accepts(Type type) => true;
    
    public object? ReadYaml(IParser parser, Type type)
    {
        var result = ParseNode(parser);
        return result;
    }
    
    public void WriteYaml(IEmitter emitter, object? value, Type type) { }

    private object ParseNode(IParser parser)
    {
        var node = parser.Current switch
        {
            MappingStart x => ParseMap(parser, x),
            SequenceStart x => ParseList(parser, x),
            Scalar x => ParseScalar(parser, x),
            _ => throw new Exception("Unsupported YAML structure.")
        };
        return node;
    }
    
    private object ParseScalar(IParser parser, Scalar e)
    {
        var str = e.Value;
        var tag = Tag(e);
        object scalar = tag switch
        {
            _ when e.IsKey => str,
            "!Duration" => TimeSpan.Parse(str),
            "!!set" => str,
            _ when int.TryParse(str, out var i) => i,
            _ when double.TryParse(str, out var d) => d
        };
        return scalar;
    }

    private string? Tag(NodeEvent e) =>
        e.Tag.IsEmpty ? null : e.Tag.Value;
    
    private Dictionary<string, object> ParseMap(IParser parser, MappingStart e)
    {
        var map = new Dictionary<string, object>();
        parser.MoveNext();
        while (parser.Current?.GetType() != typeof(MappingEnd))
        {
            var tag = Tag(e);
            var key = (parser.Current as Scalar)!.Value;
            parser.MoveNext();
            var value = ParseNode(parser);
            parser.MoveNext();
            map[key] = value;
        }

        return map;
    }
    
    private List<object> ParseList(IParser parser, SequenceStart e)
    {
        var list = new List<object>();
        parser.MoveNext();
        while (parser.Current?.GetType() != typeof(SequenceEnd))
        {
            var item = ParseNode(parser);
            var tag = Tag(e);
            parser.MoveNext();
            list.Add(item);
        }
        return list;
    }
}

public static class Yaml
{
    private static IDeserializer? _deserializer;
    public static IDeserializer Deserializer => _deserializer ??= GetDeserializer();
    private static IDeserializer GetDeserializer()
    {
        var converter = new YamlReader();
        var result =
            new DeserializerBuilder()
                .WithTagMapping("!Test", typeof(object))
                .WithTagMapping("!Result", typeof(object))
                .WithTagMapping("!SolutionSet", typeof(object))
                .WithTagMapping("!Solution", typeof(object))
                .WithTagMapping("!Duration", typeof(object))
                .WithTypeConverter(converter)
                .Build();
        return result;
    }

    public static object ParseString(string s)
    {
        var node = Deserializer.Deserialize(s);
        return node;
    }
    
    public static object ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }
    
}