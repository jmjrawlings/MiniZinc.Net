﻿namespace MiniZinc.Parser;

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
public sealed class MiniZincData(IReadOnlyDictionary<string, Datum>? dict = null)
    : IEquatable<IReadOnlyDictionary<string, Datum>>,
        IReadOnlyDictionary<string, Datum>
{
    private readonly IReadOnlyDictionary<string, Datum> _dict =
        dict ?? new Dictionary<string, Datum>();

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public string SourceText => Write(WriteOptions.Minimal);

    public bool Equals(IReadOnlyDictionary<string, Datum>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // TODO - faster/better
        foreach (var kv in _dict)
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
            if (!_dict.ContainsKey(name))
                return false;
        }

        return true;
    }

    public IEnumerator<KeyValuePair<string, Datum>> GetEnumerator() => _dict.GetEnumerator();

    public override bool Equals(object? obj) => Equals(obj as IReadOnlyDictionary<string, Datum>);

    public override int GetHashCode() => SourceText.GetHashCode();

    public bool TryGetValue(string key, [NotNullWhen(true)] out Datum? value) =>
        _dict.TryGetValue(key, out value);

    public bool ContainsKey(string key) => _dict.ContainsKey(key);

    public Datum this[string name] => _dict[name];

    public IEnumerable<string> Keys => _dict.Keys;

    public IEnumerable<Datum> Values => _dict.Values;

    public int Count => _dict.Count;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => SourceText;
}
