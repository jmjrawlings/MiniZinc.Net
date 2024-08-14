namespace MiniZinc.Parser;

public sealed class IntSet : SetDatum<int>
{
    public IntSet(List<int> values)
        : base(values) { }

    public override bool Equals(object? obj)
    {
        if (obj is IntRange range)
        {
            if (Count is 0)
                return false;

            if (Count > range.Count)
                return false;

            int i = 0;
            for (int v = range.Lower; v <= range.Upper; v++)
            {
                if (_values[i++] != v)
                    return false;
            }

            return true;
        }
        else
        {
            return base.Equals(obj);
        }
    }
}
