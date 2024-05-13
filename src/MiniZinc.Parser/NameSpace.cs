namespace MiniZinc.Parser;

using System.Collections;

/// <summary>
/// A lookup from names to values.  This differs from
/// a standard Dictionary in that it stores a stack for
/// each name to allow temporary shadowing eg: a local
/// namespace overriding some variables in a global one
/// </summary>
public sealed class NameSpace<T> : IReadOnlyDictionary<string, T>
{
    private Dictionary<string, T> _dict;

    public NameSpace(in IReadOnlyDictionary<string, T>? old = null)
    {
        if (old is not null)
            _dict = new Dictionary<string, T>(old);
        else
            _dict = new Dictionary<string, T>();
    }

    /// Push a binding onto the namespace
    public T? Add(in IBinding<T> b)
    {
        var name = b.Name;
        var val = b.Value;
        var existing = Add(name, val);
        return existing;
    }

    /// Push a binding onto the namespace
    public T? Add(string name, T value)
    {
        if (_dict.TryGetValue(name, out var existing))
        {
            _dict[name] = value;
            return existing;
        }

        _dict[name] = value;
        return default;
    }

    public int Count => _dict.Count;

    public bool ContainsKey(string key)
    {
        return _dict.ContainsKey(key);
    }

    public bool TryGetValue(string key, out T value) => _dict.TryGetValue(key, out value!);

    public T this[string key] => _dict[key];

    public IEnumerable<string> Keys => _dict.Keys;

    public IEnumerable<string> Names => _dict.Keys;

    public IEnumerable<T> Values => _dict.Values;

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _dict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dict).GetEnumerator();
    }

    public override string ToString()
    {
        return $"NameSpace ({Count})";
    }
}
