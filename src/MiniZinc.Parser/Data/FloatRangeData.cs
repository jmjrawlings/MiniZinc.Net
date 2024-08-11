namespace MiniZinc.Parser;

public sealed class FloatRangeData : DataNode
{
    public readonly decimal Lower;
    public readonly decimal Upper;

    public FloatRangeData(decimal lower, decimal upper)
    {
        Lower = lower;
        Upper = upper;
    }

    public override bool Equals(object? obj)
    {
        if (obj is FloatRangeData range)
        {
            if (!Lower.Equals(range.Lower))
                return false;

            if (!Upper.Equals(range.Upper))
                return false;

            return true;
        }
        else if (obj is FloatSetData set)
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