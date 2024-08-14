namespace MiniZinc.Parser;

using System.Text.Json;
using System.Text.Json.Nodes;

/// <summary>
/// The values that that appear in MiniZinc data files or
/// syntax.
/// </summary>
public abstract class MiniZincDatum
{
    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteValue(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public static readonly MiniZincDatum Empty = new EmptyDatum();

    public static readonly MiniZincDatum True = new BoolDatum(true);

    public static readonly MiniZincDatum False = new BoolDatum(false);

    public static MiniZincDatum Float(decimal f) => new FloatDatum(f);

    public static MiniZincDatum Int(int i) => new IntDatum(i);

    public static MiniZincDatum String(string s) => new StringDatum(s);

    public static MiniZincDatum FromJson(JsonNode? node)
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

    public static MiniZincDatum FromJson(JsonObject obj)
    {
        Dictionary<string, MiniZincDatum> dict = [];
        foreach (var (key, node) in obj)
        {
            var value = FromJson(node);
            dict[key] = value;
        }

        var data = new RecordDatum(dict);
        return data;
    }

    public static MiniZincDatum FromJson(JsonArray array)
    {
        List<MiniZincDatum> items = [];
        foreach (var node in array)
        {
            var item = FromJson(node);
            items.Add(item);
        }
        var data = new DatumArray(items);
        return data;
    }

    public static MiniZincDatum FromJson(JsonValue node) =>
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
