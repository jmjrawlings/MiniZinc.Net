namespace MiniZinc.Parser;

public sealed class IntRangeData : DataNode
{
    public readonly int Lower;
    public readonly int Upper;
    public readonly int Size;
    public readonly int Count;

    public IntRangeData(int lower, int upper)
    {
        Lower = lower;
        Upper = upper;
        Size = upper - lower;
        Count = Size + 1;
    }

    public override bool Equals(object? obj)
    {
        if (obj is IntRangeData range)
        {
            if (!Lower.Equals(range.Lower))
                return false;
            if (!Upper.Equals(range.Upper))
                return false;
            return true;
        }
        else if (obj is IntSetData set)
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
}