namespace MiniZinc.Parser;

using System.Collections;

public sealed class IntRange : Datum, IEnumerable<int>
{
    public readonly int Lower;
    public readonly int Upper;
    public readonly int Size;
    public readonly int Count;

    public override DatumKind Kind => DatumKind.Set;

    public IntRange(int lower, int upper)
    {
        Lower = lower;
        Upper = upper;
        Size = upper - lower;
        Count = Size + 1;
    }

    public bool Contains(int i) => i >= Lower && i <= Upper;

    public IEnumerable<int> Enumerate()
    {
        for (int i = Lower; i <= Upper; i++)
            yield return i;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IntRange range)
        {
            if (!Lower.Equals(range.Lower))
                return false;
            if (!Upper.Equals(range.Upper))
                return false;
            return true;
        }
        else if (obj is IntSet set)
        {
            if (!set.Equals(this))
                return false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public IEnumerator<int> GetEnumerator() => Enumerate().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
