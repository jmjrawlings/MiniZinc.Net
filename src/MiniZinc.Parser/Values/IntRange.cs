namespace MiniZinc.Parser;

public sealed class IntRange(int lower, int upper) : ValueSyntax(default)
{
    public int Lower => lower;
    public int Upper => upper;
}

public sealed class FloatRange(decimal lower, decimal upper) : ValueSyntax(default)
{
    public decimal Lower => lower;
    public decimal Upper => upper;
}
