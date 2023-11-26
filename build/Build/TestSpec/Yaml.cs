namespace MiniZinc.Build;

using System.Collections;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public enum YamlKind
{
    Value,
    Seq,
    Map,
    None
}

public sealed class YamlNode : IEnumerable<YamlNode>
{
    /// Yaml tag, if any (eg: !Test)
    public string? Tag { get; set; }

    /// Key, if it is an item in a map
    public string? Key { get; set; }

    public YamlKind Kind { get; private set; } = YamlKind.None;

    List<YamlNode>? _seq;

    public bool IsNone => Kind == YamlKind.None;

    public int? Int { get; }

    public double? Double { get; }

    public string? String { get; }

    public TimeSpan? Duration { get; }

    public bool? Bool { get; }

    public YamlNode() { }

    public YamlNode(Scalar scalar)
    {
        Kind = YamlKind.Value;
        Tag = Yaml.GetTag(scalar);
        String = scalar.Value;
        if (Tag == "DURATION")
            Duration = TimeSpan.Parse(String);
        else if (String == "true")
            Bool = true;
        else if (String == "false")
            Bool = false;
        else if (int.TryParse(String, out var i))
            Int = i;
        else if (double.TryParse(String, out var d))
            Double = d;
    }

    public YamlNode this[string key]
    {
        get
        {
            foreach (var item in this)
                if (item.Key == key)
                    return item;
            return new YamlNode();
        }
    }

    public IEnumerator<YamlNode> GetEnumerator()
    {
        if (_seq is null)
            yield break;
        foreach (var item in _seq!)
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(string key, YamlNode value)
    {
        value.Key = key;
        Add(value);
        Kind = YamlKind.Map;
    }

    public void Add(YamlNode node)
    {
        _seq ??= new List<YamlNode>();
        _seq.Add(node);
        Kind = YamlKind.Seq;
    }
}

public static class Yaml
{
    public static YamlNode? ParseString(string s) => Parser.ParseString(s);

    public static YamlNode? ParseFile(FileInfo fi) => Parser.ParseFile(fi);

    private static YamlParser? _parser;
    private static YamlParser Parser => _parser ??= new YamlParser();

    internal static string? GetTag(NodeEvent e)
    {
        if (e.Tag.IsEmpty)
            return null;
        var s = e.Tag.Value.Where(char.IsLetter);
        var x = string.Join("", s).ToUpper();
        Console.WriteLine(x);
        return x;
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

    private YamlNode ParseNode(IParser parser)
    {
        var curr = parser.Current;
        // Console.WriteLine("{0} {1} {2}", curr.Start, curr.End, curr.GetType().Name);
        var node = curr switch
        {
            MappingStart x => ParseMap(parser, x),
            SequenceStart x => ParseList(parser, x),
            Scalar x => new YamlNode(x),
            _ => throw new Exception($"Unexpected yaml event {curr}")
        };
        parser.MoveNext();
        return node;
    }

    private YamlNode ParseMap(IParser parser, MappingStart e)
    {
        var map = new YamlNode();
        parser.MoveNext();
        loop:
        var curr = parser.Current;
        var tag = Yaml.GetTag(e);
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

        map.Tag = tag;
        return map;
    }

    private YamlNode ParseList(IParser parser, SequenceStart e)
    {
        parser.MoveNext();
        var node = new YamlNode();
        loop:
        var curr = parser.Current;
        var tag = Yaml.GetTag(e);
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
        node.Tag = tag;
        return node;
    }
}

internal sealed class YamlParser
{
    private readonly IDeserializer _deserializer;

    public YamlParser()
    {
        var converter = new YamlConverter();
        _deserializer = new DeserializerBuilder()
            .WithTagMapping("!Test", typeof(object))
            .WithTagMapping("!Result", typeof(object))
            .WithTagMapping("!SolutionSet", typeof(object))
            .WithTagMapping("!Solution", typeof(object))
            .WithTagMapping("!Duration", typeof(object))
            .WithTypeConverter(converter)
            .Build();
    }

    public YamlNode? ParseString(string s)
    {
        var text = s.TrimEnd();
        var node = _deserializer.Deserialize(text) as YamlNode;
        return node;
    }

    public YamlNode? ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }
}
