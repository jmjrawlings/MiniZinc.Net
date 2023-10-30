namespace MiniZinc.Tests;

using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public static class Yaml
{
    public static T? ParseString<T>(string s)
        where T : YamlNode => Parser.ParseString<T>(s);

    public static T? ParseFile<T>(FileInfo fi)
        where T : YamlNode => Parser.ParseFile<T>(fi);

    private static YamlParser? _parser;
    private static YamlParser Parser => _parser ??= new YamlParser();
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
            Scalar x => ParseScalar(parser, x),
            _ => throw new Exception($"Unexpected yaml event {curr}")
        };
        parser.MoveNext();
        return node;
    }

    private YamlNode ParseScalar(IParser parser, Scalar e)
    {
        var str = e.Value;
        var tag = GetTag(e);
        YamlNode scalar = tag switch
        {
            "!!set" => Token(str),
            "!Duration" => Token(TimeSpan.Parse(str)),
            _ when str == "true" => Token(true),
            _ when str == "false" => Token(false),
            _ when int.TryParse(str, out var i) => Token(i),
            _ when double.TryParse(str, out var d) => Token(d),
            _ => Token(str)
        };
        return scalar;
    }

    private string? GetTag(NodeEvent e)
    {
        if (e.Tag.IsEmpty)
            return null;
        var s = e.Tag.Value.Where(char.IsLetter);
        var x = string.Join("", s).ToUpper();
        Console.WriteLine(x);
        return x;
    }

    private YamlMap ParseMap(IParser parser, MappingStart e)
    {
        var map = Map();
        var tag = GetTag(e);
        parser.MoveNext();
        loop:
        var curr = parser.Current;
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
                map.Dict[key] = value;
                goto loop;
        }

        map.Tag = tag;
        return map;
    }

    private YamlNode ParseList(IParser parser, SequenceStart e)
    {
        parser.MoveNext();
        var seq = Seq();
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
                seq.List.Add(item);
                goto loop;
        }
        seq.Tag = tag;
        return seq;
    }

    private static YamlToken<T> Token<T>(T value) => new(value);

    private static YamlSequence Seq() => new();

    private static YamlMap Map() => new();
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

    public T? ParseString<T>(string s)
        where T : YamlNode
    {
        var text = s.TrimEnd();
        var obj = _deserializer.Deserialize(text);
        var yaml = obj as T;
        return yaml;
    }

    public T? ParseFile<T>(FileInfo fi)
        where T : YamlNode
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString<T>(text);
        return node;
    }
}

public abstract class YamlNode
{
    public string? Tag { get; set; }

    public YamlNode? Get(string key) => Map.Dict.TryGet(key);

    public int Int => (this as YamlToken<int>)!.Value;

    public string String => (this as YamlToken<string>)!.Value;

    public TimeSpan Duration => (this as YamlToken<TimeSpan>)!.Value;

    public bool Bool => (this as YamlToken<bool>)!.Value;

    public YamlMap Map => (YamlMap)this;

    public List<T> ListOf<T>(Func<YamlNode, T> f) => ((YamlSequence)this).List.Select(f).ToList();

    public List<T> ListOf<T>(string key, Func<YamlNode, T> f) =>
        Get(key)?.ListOf(f) ?? Enumerable.Empty<T>().ToList();

    public Dictionary<string, T> DictOf<T>(Func<YamlNode, T> f)
    {
        var dict = new Dictionary<string, T>();
        foreach (var kv in (this as YamlMap)!.Dict)
            dict[kv.Key] = f(kv.Value);
        return dict;
    }

    public Dictionary<string, T> DictOf<T>(string key, Func<YamlNode, T> f) =>
        Get(key)?.DictOf(f) ?? new Dictionary<string, T>();
}

public sealed class YamlToken<T> : YamlNode
{
    public readonly T Value;

    public static implicit operator T(YamlToken<T> d) => d.Value;

    public YamlToken(T value)
    {
        Value = value;
    }
}

public sealed class YamlMap : YamlNode
{
    public Dictionary<string, YamlNode> Dict { get; } = new();
}

public sealed class YamlSequence : YamlNode
{
    public List<YamlNode> List { get; } = new();
}
