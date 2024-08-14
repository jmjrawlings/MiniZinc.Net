namespace MiniZinc.Parser;

using System.Collections;

public abstract class SetDatum<T> : Datum, IReadOnlyList<T>
{
    protected readonly List<T> _values;

    public override DatumKind Kind => DatumKind.Set;

    protected SetDatum(List<T> values)
    {
        _values = values;
    }

    public IEnumerator<T> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _values.Count;

    public T this[int index] => _values[index];

    public bool Equals(SetDatum<T>? other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        if (Count is 0)
            return true;

        for (int i = 0; i < Count; i++)
        {
            T a = this[i];
            T b = other[i];
            if (ReferenceEquals(a, b))
                continue;
            if (!a.Equals(b))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as SetDatum<T>);
}

public sealed class SetDatum(List<Datum> values) : SetDatum<Datum>(values) { }
