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
public sealed class DataSyntax(Dictionary<string, ValueSyntax> values)
    : IEquatable<IReadOnlyDictionary<string, ValueSyntax>>
{
    public IReadOnlyDictionary<string, ValueSyntax> Values => values;

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteData(this);
        var mzn = writer.ToString();
        return mzn;
    }

    public override string ToString()
    {
        var mzn = Write(WriteOptions.Minimal);
        return mzn;
    }

    public bool Equals(IReadOnlyDictionary<string, ValueSyntax>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // TODO - faster/better
        foreach (var kv in values)
        {
            var name = kv.Key;
            var a = kv.Value;

            // Variable does not exist in B
            if (!other!.TryGetValue(name, out var b))
                return false;

            // Variable exist in B but is different
            if (!a.Equals(b))
                return false;
        }

        // Check surplus variables in B
        foreach (var kv in other)
        {
            var name = kv.Key;
            if (!values.ContainsKey(name))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (!Equals(obj as IReadOnlyDictionary<string, ExpressionSyntax>))
            return false;

        return true;
    }
}
