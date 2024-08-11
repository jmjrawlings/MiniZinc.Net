namespace MiniZinc.Parser;

public sealed class FloatSetData(List<decimal> values) : SetData<decimal>(values)
{
    public override bool Equals(object? obj)
    {
        if (obj is FloatRangeData { Lower: var lo, Upper: var hi })
        {
            // Can only compare float set/range equality for the singleton set
            if (values is not [var value])
                return false;

            if (!lo.Equals(value))
                return false;

            if (!hi.Equals(value))
                return false;

            return true;
        }
        else
        {
            return base.Equals(obj);
        }
    }
}