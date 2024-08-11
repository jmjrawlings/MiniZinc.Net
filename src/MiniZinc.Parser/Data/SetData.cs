namespace MiniZinc.Parser;

using System.Collections;

public abstract class SetData<T>(List<T> values) : DataNode, IReadOnlyList<T>
{
    public IEnumerator<T> GetEnumerator() => values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => values.Count;

    public T this[int index] => values[index];

    public bool Equals(SetData<T>? other)
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

    public override bool Equals(object? obj) => Equals(obj as SetData<T>);
}

public sealed class SetData(List<DataNode> values) : SetData<DataNode>(values) { }
