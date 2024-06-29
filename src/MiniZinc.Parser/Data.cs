namespace MiniZinc.Parser;

using System.Collections;
using Syntax;

/// <summary>
/// Result of parsing a minizinc data from a file (.dzn) or string
/// </summary>
public sealed class Data
    : IReadOnlyDictionary<string, ExpressionSyntax>,
        IEquatable<IReadOnlyDictionary<string, ExpressionSyntax>>
{
    private readonly Dictionary<string, ExpressionSyntax> _variables;

    public Data()
    {
        _variables = new Dictionary<string, ExpressionSyntax>();
    }

    public Data(IReadOnlyDictionary<string, ExpressionSyntax> variables)
    {
        _variables = new Dictionary<string, ExpressionSyntax>(variables);
    }

    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        foreach (var expr in _variables.Values)
            writer.WriteNode(expr);
        var mzn = writer.ToString();
        return mzn;
    }

    public IEnumerator<KeyValuePair<string, ExpressionSyntax>> GetEnumerator()
    {
        return _variables.GetEnumerator();
    }

    public override string ToString()
    {
        var mzn = Write(WriteOptions.Minimal);
        return mzn;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_variables).GetEnumerator();
    }

    public int Count => _variables.Count;

    public bool ContainsKey(string key)
    {
        return _variables.ContainsKey(key);
    }

    public bool TryGetValue(string key, out ExpressionSyntax value)
    {
        return _variables.TryGetValue(key, out value);
    }

    public ExpressionSyntax this[string key] => _variables[key];

    public IEnumerable<string> Keys => _variables.Keys;

    public IEnumerable<ExpressionSyntax> Values => _variables.Values;

    public bool Equals(IReadOnlyDictionary<string, ExpressionSyntax>? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        // TODO - faster/better
        foreach (var kv in _variables)
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
            if (!ContainsKey(name))
                return false;
        }

        return true;
    }

    public bool Remove(string key) => _variables.Remove(key);

    public override bool Equals(object? obj)
    {
        if (!Equals(obj as IReadOnlyDictionary<string, ExpressionSyntax>))
            return false;

        return true;
    }
}
