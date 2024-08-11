namespace MiniZinc.Parser;

using System.Text.Json;
using System.Text.Json.Nodes;

public abstract class DataNode
{
    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteValue(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public static readonly DataNode Empty = new EmptyData();

    public static readonly DataNode True = new BoolData(true);

    public static readonly DataNode False = new BoolData(false);

    public static DataNode Float(decimal f) => new FloatData(f);

    public static DataNode Int(int i) => new IntData(i);

    public static DataNode String(string s) => new StringData(s);

    public static DataNode FromJson(JsonNode? node)
    {
        switch (node)
        {
            case JsonArray array:
                return FromJson(array);
            case JsonObject obj:
                return FromJson(obj);
            case JsonValue val:
                return FromJson(val);
            default:
                return Empty;
        }
    }

    public static DataNode FromSyntax(JsonObject obj)
    {
        Dictionary<string, DataNode> dict = [];
        foreach (var (key, node) in obj)
        {
            var value = FromJson(node);
            dict[key] = value;
        }

        var data = new RecordData(dict);
        return data;
    }

    public static DataNode FromSyntax(JsonArray array)
    {
        List<DataNode> items = new();
        foreach (var node in array)
        {
            var item = FromJson(node);
            items.Add(item);
        }
        var data = new ArrayData(items);
        return data;
    }

    public static DataNode FromSyntax(JsonValue node) =>
        node.GetValueKind() switch
        {
            JsonValueKind.Null => Empty,
            JsonValueKind.True => True,
            JsonValueKind.False => False,
            JsonValueKind.Number when node.TryGetValue<decimal>(out var dec) => Float(dec),
            JsonValueKind.Number when node.TryGetValue<int>(out var i) => Int(i),
            JsonValueKind.String => String(node.GetValue<string>()),
            _ => throw new ArgumentException()
        };
}

public sealed class IntData(int value) : DataNode
{
    public int Value => value;

    public static implicit operator int(IntData expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not int other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}

public sealed class BoolData(bool value) : DataNode
{
    public bool Value => value;

    public static implicit operator bool(BoolData expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not bool other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}

public sealed class FloatData(decimal value) : DataNode
{
    public decimal Value => value;

    public static implicit operator decimal(FloatData expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not decimal other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}

public sealed class StringData(string value) : DataNode
{
    public string Value => value;

    public static implicit operator string(StringData expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not string other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value;
}

public sealed class EmptyData : DataNode
{
    public override bool Equals(object? obj) => obj is EmptyData;
}
