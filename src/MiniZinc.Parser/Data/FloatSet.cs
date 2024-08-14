namespace MiniZinc.Parser;

public sealed class FloatSet : SetDatum<decimal>
{
    public FloatSet(List<decimal> values)
        : base(values) { }

    public override bool Equals(object? obj)
    {
        if (obj is FloatRange { Lower: var lo, Upper: var hi })
        {
            // Can only compare float set/range equality for the singleton set
            if (_values is not [var value])
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
