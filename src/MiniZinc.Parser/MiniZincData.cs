namespace MiniZinc.Parser;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MiniZinc;

/// <summary>
/// Result of parsing minizinc data from a file (.dzn) or string.
/// </summary>
/// <remarks>
/// This is different from a <see cref="MiniZincModel"/> in that it can only
/// contain assignments of the form `$name = $datum;`
/// </remarks>
[DebuggerDisplay("{SourceText}")]
public sealed class MiniZincData
    : IEquatable<IDictionary<string, MiniZincExpr>>,
        IDictionary<string, MiniZincExpr>
{
    private readonly IDictionary<string, MiniZincExpr> _data;

    /// <summary>
    /// Result of parsing minizinc data from a file (.dzn) or string.
    /// </summary>
    /// <remarks>
    /// This is different from a <see cref="MiniZincModel"/> in that it can only
    /// contain assignments of the form `$name = $datum;`
    /// </remarks>
    public MiniZincData(IDictionary<string, MiniZincExpr>? data = null)
    {
        _data = data ?? new Dictionary<string, MiniZincExpr>();
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public string SourceText => Write(WriteOptions.Minimal);

    public bool Equals(IDictionary<string, MiniZincExpr>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // TODO - faster/better
        foreach (var kv in _data)
        {
            var name = kv.Key;
            var a = kv.Value;

            // Variable does not exist in B
            if (!other.TryGetValue(name, out var b))
                return false;

            // Variable exist in B but is different
            if (!a.Equals(b))
                return false;
        }

        // Check surplus variables in B
        foreach (var kv in other)
        {
            var name = kv.Key;
            if (!_data.ContainsKey(name))
                return false;
        }

        return true;
    }

    public IEnumerator<KeyValuePair<string, MiniZincExpr>> GetEnumerator() => _data.GetEnumerator();

    public override bool Equals(object? obj) => Equals(obj as IDictionary<string, MiniZincExpr>);

    public override int GetHashCode() => SourceText.GetHashCode();

    public void Add(string key, MiniZincExpr value)
    {
        _data.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return _data.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out MiniZincExpr value)
    {
        return _data.TryGetValue(key, out value);
    }

    public T Get<T>(string key)
    {
        if (!_data.TryGetValue(key, out var expr))
            throw new KeyNotFoundException($"Data did not contain an entry for \"{key}\".");

        if (expr is not T datum)
            throw new InvalidCastException($"Could not cast \"{key}\" from {expr} to {nameof(T)}.");

        return datum;
    }

    public MiniZincExpr this[string name]
    {
        get => _data[name];
        set => throw new NotImplementedException();
    }

    public ICollection<string> Keys => _data.Keys;

    public ICollection<MiniZincExpr> Values => _data.Values;

    public override string ToString() => SourceText;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_data).GetEnumerator();
    }

    public void Add(KeyValuePair<string, MiniZincExpr> item)
    {
        _data.Add(item);
    }

    public void Clear()
    {
        _data.Clear();
    }

    public bool Contains(KeyValuePair<string, MiniZincExpr> item)
    {
        return _data.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, MiniZincExpr>[] array, int arrayIndex)
    {
        _data.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, MiniZincExpr> item)
    {
        return _data.Remove(item);
    }

    public int Count => _data.Count;

    public bool IsReadOnly => _data.IsReadOnly;
}
