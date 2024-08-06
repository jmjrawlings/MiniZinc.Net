namespace MiniZinc.Parser;

using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Syntax;

/// <summary>
/// Result of parsing a minizinc data from a file (.dzn) or string.
/// </summary>
/// <remarks>
/// Data is different from a <see cref="MiniZincModel"/> in that it can only
/// contain assignments of the form `$name = $expr;`
/// </remarks>
[DebuggerDisplay("{SourceText}")]
public sealed class MiniZincData(Dictionary<string, DataSyntax> dict)
    : IEquatable<IReadOnlyDictionary<string, DataSyntax>>,
        IReadOnlyDictionary<string, DataSyntax>
{
    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public string SourceText => Write(WriteOptions.Minimal);

    public override string ToString() => SourceText;

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Equals(IReadOnlyDictionary<string, DataSyntax>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // TODO - faster/better
        foreach (var kv in dict)
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
            if (!dict.ContainsKey(name))
                return false;
        }

        return true;
    }

    public IEnumerator<KeyValuePair<string, DataSyntax>> GetEnumerator() => dict.GetEnumerator();

    public override bool Equals(object? obj) =>
        Equals(obj as IReadOnlyDictionary<string, DataSyntax>);

    public override int GetHashCode() => SourceText.GetHashCode();

    /// <summary>
    /// Get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    /// <exception cref="Exception">The variable does not exists or was not of the expected type</exception>
    public U Get<U>(string id)
        where U : DataSyntax
    {
        if (TryGet<U>(id) is not { } value)
            throw new KeyNotFoundException($"Result did not contain a solution for \"{id}\"");

        return value;
    }

    public DataSyntax? TryGet(string id) => dict.GetValueOrDefault(id);

    /// <summary>
    /// Try to get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    public U? TryGet<U>(string id)
        where U : DataSyntax
    {
        var value = TryGet(id);
        if (value is null)
            return null;

        if (value is not U u)
            throw new Exception();

        return u;
    }

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out DataSyntax? value) =>
        dict.TryGetValue(key, out value);

    public DataSyntax this[string name] => Get<DataSyntax>(name);

    public IEnumerable<string> Keys => dict.Keys;
    public IEnumerable<DataSyntax> Values => dict.Values;

    public int Count => dict.Count;
}
