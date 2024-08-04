using System.Diagnostics;

namespace MiniZinc.Parser;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Syntax;

/// <summary>
/// Result of parsing a minizinc data from a file (.dzn) or string.
/// </summary>
/// <remarks>
/// Data is different from a <see cref="ModelSyntax"/> in that it can only
/// contain assignments of the form `$name = $expr;`
/// </remarks>
[DebuggerDisplay("{SourceText}")]
public sealed class DataSyntax(Dictionary<string, ValueSyntax> dict)
    : IEquatable<IReadOnlyDictionary<string, ValueSyntax>>,
        IReadOnlyDictionary<string, ValueSyntax>
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

    public bool Equals(IReadOnlyDictionary<string, ValueSyntax>? other)
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

    public IEnumerator<KeyValuePair<string, ValueSyntax>> GetEnumerator() => dict.GetEnumerator();

    public override bool Equals(object? obj) =>
        Equals(obj as IReadOnlyDictionary<string, ExpressionSyntax>);

    public override int GetHashCode() => SourceText.GetHashCode();

    /// <summary>
    /// Get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    /// <exception cref="Exception">The variable does not exists or was not of the expected type</exception>
    public U Get<U>(string id)
        where U : ExpressionSyntax
    {
        if (TryGet<U>(id) is not { } value)
            throw new KeyNotFoundException($"Result did not contain a solution for \"{id}\"");

        return value;
    }

    public ValueSyntax? TryGet(string id) => dict.GetValueOrDefault(id);

    /// <summary>
    /// Try to get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    public U? TryGet<U>(string id)
        where U : ExpressionSyntax
    {
        var value = TryGet(id);
        if (value is null)
            return null;

        if (value is not U u)
            throw new Exception();

        return u;
    }

    public bool ContainsKey(string key) => dict.ContainsKey(key);

    public bool TryGetValue(string key, [NotNullWhen(true)] out ValueSyntax? value) =>
        dict.TryGetValue(key, out value);

    public ValueSyntax this[string name] => Get<ValueSyntax>(name);

    public IEnumerable<string> Keys => dict.Keys;
    public IEnumerable<ValueSyntax> Values => dict.Values;

    /// Get the int solution for the given variable
    public int GetInt(string id) => Get<IntLiteralSyntax>(id);

    /// Get the bool solution for the given variable
    public bool GetBool(string id) => Get<BoolLiteralSyntax>(id);

    /// Get the float solution for the given variable
    public decimal GetFloat(string id) => Get<FloatLiteralSyntax>(id);

    /// Get the array solution for the given variable
    public IEnumerable<U> GetArray1D<U>(string id)
    {
        var array = Get<Array1dValueSyntax>(id);
        foreach (var node in array.Values)
        {
            if (node is not ValueSyntax<U> literal)
                throw new Exception();
            yield return literal.Value;
        }
    }

    public int Count => dict.Count;
}
