namespace MiniZinc.Parser;

using System.Collections;
using System.Collections.Immutable;

/// <summary>
/// A lookup from names to values.  This differs from
/// a standard Dictionary in that it stores a stack for
/// each name to allow temporary shadowing eg: a local
/// namespace overriding some variables in a global one
/// </summary>
public sealed class NameSpace<T> : IReadOnlyDictionary<string, T>
{
    /// The order in which names were edited
    private readonly Stack<string> _log;

    /// Each variable holds a history of its values
    private readonly Dictionary<string, Stack<T>> _bindings;

    /// The current scope
    private readonly Dictionary<string, T> _scope;

    public NameSpace(Dictionary<string, T>? scope = null)
    {
        _log = new();
        _bindings = new();
        if (scope is not null)
            _scope = new Dictionary<string, T>(scope);
        else
            _scope = new Dictionary<string, T>();
    }

    /// Push a binding onto the namespace
    public void Push(in IBinding<T> bnd)
    {
        var name = bnd.Name;
        var val = bnd.Value;
        Push(name, val);
    }

    /// Push a binding onto the namespace
    public void Push(string name, T value)
    {
        if (!_bindings.TryGetValue(name, out var stack))
        {
            stack = new Stack<T>();
            _bindings[name] = stack;
        }
        stack.Push(value);
        _scope[name] = value;
        _log.Push(name);
    }

    /// Pop the last binding off the namespace
    public Binding<T> Pop()
    {
        var name = _log.Pop();
        var stack = _bindings[name];
        var value = stack.Pop();
        if (stack.Count == 0)
        {
            _bindings.Remove(name);
            _scope.Remove(name);
        }
        else
        {
            _scope[name] = stack.Peek();
        }

        var binding = name.Bind(value);
        return binding;
    }

    public int Count => _scope.Count;

    public bool ContainsKey(string key)
    {
        return _scope.ContainsKey(key);
    }

    public bool TryGetValue(string key, out T value) => _scope.TryGetValue(key, out value);

    public T this[string key] => _scope[key];
    public IEnumerable<string> Keys => _scope.Keys;

    /// Returns the names with duplicate values assigned
    public IEnumerable<Binding<IEnumerable<T>>> Bindings
    {
        get
        {
            foreach (var kv in _bindings)
                if (kv.Value.Count > 1)
                {
                    IEnumerable<T> seq = kv.Value.ToImmutableArray();
                    var bnd = kv.Key.Bind(seq);
                    yield return bnd;
                }
        }
    }

    public IEnumerable<string> Names => _bindings.Keys;

    public IEnumerable<T> Values
    {
        get
        {
            foreach (var kv in _bindings)
                yield return kv.Value.Peek();
        }
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _scope.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_scope).GetEnumerator();
    }

    public override string ToString()
    {
        return $"NameSpace ({Count})";
    }
}
