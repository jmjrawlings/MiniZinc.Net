namespace MiniZinc.Parser;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// MiniZinc record literal
/// </summary>
/// <mzn>(bool: a, int: c, float: f)</mzn>
public sealed class RecordData(Dictionary<string, DataNode> dict)
    : DataNode,
        IReadOnlyDictionary<string, DataNode>
{
    public IEnumerator<KeyValuePair<string, DataNode>> GetEnumerator() => dict.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => dict.Count;

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out DataNode? value) =>
        dict.TryGetValue(key, out value);

    public DataNode this[string key] => dict[key];

    public IEnumerable<string> Keys => dict.Keys;

    public IEnumerable<DataNode> Values => dict.Values;

    public bool Equals(IReadOnlyDictionary<string, DataNode>? other)
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

    public override bool Equals(object? obj) =>
        Equals(obj as IReadOnlyDictionary<string, DataNode>);
}
