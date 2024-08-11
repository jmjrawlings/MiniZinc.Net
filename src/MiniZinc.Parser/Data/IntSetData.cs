namespace MiniZinc.Parser;

public sealed class IntSetData(List<int> values) : SetData<int>(values)
{
    public override bool Equals(object? obj)
    {
        if (obj is IntRangeData range)
        {
            if (Count is 0)
                return false;

            if (Count > range.Count)
                return false;

            int i = 0;
            for (int v = range.Lower; v <= range.Upper; v++)
            {
                if (values[i++] != v)
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
