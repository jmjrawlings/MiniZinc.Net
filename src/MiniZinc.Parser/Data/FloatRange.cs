namespace MiniZinc.Parser;

public sealed class FloatRange : Datum
{
    public readonly decimal Lower;

    public readonly decimal Upper;

    public override DatumKind Kind => DatumKind.Set;

    public FloatRange(decimal lower, decimal upper)
    {
        Lower = lower;
        Upper = upper;
    }

    public override bool Equals(object? obj)
    {
        if (obj is FloatRange range)
        {
            if (!Lower.Equals(range.Lower))
                return false;

            if (!Upper.Equals(range.Upper))
                return false;

            return true;
        }
        else if (obj is FloatSet set)
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
