namespace MiniZinc.Parser;

public sealed class Array2dCallExpr : MiniZincExpr
{
    public MiniZincExpr I { get; }
    public MiniZincExpr J { get; }

    public IReadOnlyList<MiniZincExpr> Elements { get; }

    public Array2dCallExpr(
        in Token start,
        MiniZincExpr i,
        MiniZincExpr j,
        IReadOnlyList<MiniZincExpr> elements
    )
        : base(start)
    {
        I = i;
        J = j;
        Elements = elements;
    }
}
