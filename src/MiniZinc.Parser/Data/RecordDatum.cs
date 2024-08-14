namespace MiniZinc.Parser;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// MiniZinc record literal
/// </summary>
/// <mzn>(bool: a, int: c, float: f)</mzn>
public sealed class RecordDatum(Dictionary<string, Datum> dict)
    : Datum,
        IReadOnlyDictionary<string, Datum>
{
    public IEnumerator<KeyValuePair<string, Datum>> GetEnumerator() => dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override DatumKind Kind => DatumKind.Record;

    public int Count => dict.Count;

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out Datum? value) =>
        dict.TryGetValue(key, out value);

    public Datum this[string key] => dict[key];

    public IEnumerable<string> Keys => dict.Keys;

    public IEnumerable<Datum> Values => dict.Values;

    public bool Equals(IReadOnlyDictionary<string, Datum>? other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        foreach (var (key, a) in this)
        {
            if (!other.TryGetValue(key, out var b))
                return false;
            if (!a.Equals(b))
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as IReadOnlyDictionary<string, Datum>);
}
