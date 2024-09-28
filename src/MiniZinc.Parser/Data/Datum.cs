namespace MiniZinc.Parser;

using System.Text.Json;
using System.Text.Json.Nodes;

public enum DatumKind
{
    Unknown,
    String,
    Bool,
    Int,
    Float,
    Record,
    Tuple,
    Set,
    Array,
    Empty
}

/// <summary>
/// The values that that appear in MiniZinc data files or
/// syntax.
/// </summary>
public abstract class Datum
{
    public abstract DatumKind Kind { get; }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteValue(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public static readonly Datum Empty = new EmptyDatum();

    public static readonly Datum True = new BoolDatum(true);

    public static readonly Datum False = new BoolDatum(false);

    public static Datum Bool(bool b) => new BoolDatum(b);

    public static Datum Float(decimal f) => new FloatDatum(f);

    public static Datum Int(int i) => new IntDatum(i);

    public static Datum String(string s) => new StringDatum(s);

    public static Datum FromJson(JsonNode? node)
    {
        switch (node)
        {
            case JsonArray array:
                return FromJson((JsonNode?)array);
            case JsonObject obj:
                return FromJson((JsonNode?)obj);
            case JsonValue val:
                return FromJson((JsonNode?)val);
            default:
                return Empty;
        }
    }

    public static Datum FromJson(JsonObject obj)
    {
        Dictionary<string, Datum> dict = [];
        foreach (var (key, node) in obj)
        {
            var value = FromJson(node);
            dict[key] = value;
        }

        var data = new RecordDatum(dict);
        return data;
    }

    public static Datum FromJson(JsonArray array)
    {
        List<Datum> items = [];
        foreach (var node in array)
        {
            var item = FromJson(node);
            items.Add(item);
        }
        var data = new DatumArray(items);
        return data;
    }

    public static Datum FromJson(JsonValue node) =>
        node.GetValueKind() switch
        {
            JsonValueKind.Null => Empty,
            JsonValueKind.True => True,
            JsonValueKind.False => False,
            JsonValueKind.Number when node.TryGetValue<decimal>(out var dec) => Float(dec),
            JsonValueKind.Number when node.TryGetValue<int>(out var i) => Int(i),
            JsonValueKind.String => String(node.GetValue<string>()),
            _ => throw new ArgumentException($"Could not parse {node} as a datum")
        };
}
