using System.Collections;
using System.Collections.Immutable;
using FluentAssertions.Equivalency;

namespace MiniZinc.Tests;

using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

public static class Yaml
{
    public static YamlNode ParseString(string s) => Parser.ParseString(s);

    public static YamlNode ParseFile(FileInfo fi) => Parser.ParseFile(fi);

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
                map.Add(key, value);
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
                seq.Add(item);
                goto loop;
        }
        seq.Tag = tag;
        return seq;
    }

    private static YamlToken<T> Token<T>(T value) => new(value);

    private static YamlSeq Seq() => new();

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

    public YamlNode ParseString(string s)
    {
        var text = s.TrimEnd();
        var node = _deserializer.Deserialize(text);
        if (node is YamlNode n)
            return n;
        return YamlNode.None;
    }

    public YamlNode ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }
}

public abstract class YamlNode : IEnumerable<YamlNode>
{
    public string? Tag { get; set; }

    public string? Key { get; set; }

    public bool IsNone => ReferenceEquals(this, None);

    public static readonly YamlNode None = new YamlNone();

    public YamlNode this[string key]
    {
        get
        {
            if (this is YamlMap x)
                if (x.Value.TryGetValue(key, out var node))
                    return node;
            return None;
        }
    }

    public int? Int => (this as YamlToken<int>)?.Value;

    public string? String => (this as YamlToken<string>)?.Value;

    public TimeSpan? Duration => (this as YamlToken<TimeSpan>)?.Value;

    public bool? Bool => (this as YamlToken<bool>)?.Value;

    public List<YamlNode> List() => (this as YamlSeq)?.Value ?? new List<YamlNode>();

    public List<T> List<T>(Func<YamlNode, T> f) => List().Select(f).ToList();

    public Dictionary<string, YamlNode> Dict() =>
        (this as YamlMap)?.Value ?? new Dictionary<string, YamlNode>();

    public Dictionary<string, T> Dict<T>(Func<YamlNode, T> f)
    {
        var d = new Dictionary<string, T>();
        foreach (var kv in Dict())
        {
            d[kv.Key] = f(kv.Value);
        }

        return d;
    }

    public IEnumerator<YamlNode> GetEnumerator()
    {
        if (this is YamlSeq seq)
        {
            foreach (var item in seq.Value)
                yield return item;
        }
        else if (this is YamlMap map)
        {
            foreach (var item in map.Value.Values)
                yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
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
    public readonly Dictionary<string, YamlNode> Value = new();

    public void Add(string key, YamlNode val)
    {
        val.Key = key;
        Value.Add(key, val);
    }
}

public sealed class YamlSeq : YamlNode
{
    public readonly List<YamlNode> Value = new();

    public void Add(YamlNode item)
    {
        Value.Add(item);
    }
}

public sealed class YamlNone : YamlNode { }
