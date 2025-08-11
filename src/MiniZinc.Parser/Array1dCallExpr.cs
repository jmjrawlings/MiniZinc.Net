namespace MiniZinc.Parser;

public sealed class Array1dCallExpr : MiniZincExpr
{
    public MiniZincExpr I { get; }
    public IReadOnlyList<MiniZincExpr> Elements { get; }

    public Array1dCallExpr(in Token start, MiniZincExpr i, IReadOnlyList<MiniZincExpr> elements)
        : base(start)
    {
        I = i;
        Elements = elements;
    }
}
