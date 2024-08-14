namespace MiniZinc.Parser;

using System.Collections;

public abstract class ArrayDatum<T>(List<T> values) : Datum, IReadOnlyList<T>
{
    public override DatumKind Kind => DatumKind.Array;

    public IEnumerator<T> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => values.Count;

    public T this[int index] => values[index];

    public bool Equals(ArrayDatum<T>? other)
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
            if (a != null && !a.Equals(b))
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as ArrayDatum<T>);
}

public sealed class IntArray(List<int> list) : ArrayDatum<int>(list) { }

public sealed class FloatArray(List<decimal> list) : ArrayDatum<decimal>(list) { }

public sealed class BoolArray(List<bool> list) : ArrayDatum<bool>(list) { }

public sealed class StringArray(List<string> list) : ArrayDatum<string>(list) { }

public sealed class DatumArray(List<Datum> list) : ArrayDatum<Datum>(list) { }

public sealed class DatumTuple(List<Datum> list) : ArrayDatum<Datum>(list) { }
