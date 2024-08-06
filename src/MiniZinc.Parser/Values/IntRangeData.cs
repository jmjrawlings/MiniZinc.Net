namespace MiniZinc.Parser;

public sealed class IntRangeData(int lower, int upper) : DataSyntax
{
    public int Lower => lower;
    public int Upper => upper;
}

public sealed class FloatRangeData(decimal lower, decimal upper) : DataSyntax
{
    public decimal Lower => lower;
    public decimal Upper => upper;
}
