namespace MiniZinc.Parser;

using System.Collections;

public abstract class ArrayData<T>(List<T> values) : DataNode, IReadOnlyList<T>
{
    public IEnumerator<T> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => values.Count;

    public T this[int index] => values[index];

    public bool Equals(ArrayData<T>? other)
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

    public override bool Equals(object? obj) => Equals(obj as ArrayData<T>);
}

public sealed class IntArrayData(List<int> list) : ArrayData<int>(list) { }

public sealed class FloatArrayData(List<decimal> list) : ArrayData<decimal>(list) { }

public sealed class BoolArrayData(List<bool> list) : ArrayData<bool>(list) { }

public sealed class StringArrayData(List<string> list) : ArrayData<string>(list) { }

public sealed class ArrayData(List<DataNode> list) : ArrayData<DataNode>(list) { }

public sealed class TupleData(List<DataNode> list) : ArrayData<DataNode>(list) { }
