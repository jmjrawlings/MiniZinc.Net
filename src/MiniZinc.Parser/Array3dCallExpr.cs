namespace MiniZinc.Parser;

public sealed class Array3dCallExpr : MiniZincExpr
{
    public MiniZincExpr I { get; }

    public MiniZincExpr J { get; }

    public MiniZincExpr K { get; }

    public IReadOnlyList<MiniZincExpr> Elements { get; }

    public Array3dCallExpr(
        in Token start,
        MiniZincExpr i,
        MiniZincExpr j,
        MiniZincExpr k,
        IReadOnlyList<MiniZincExpr> elements
    )
        : base(start)
    {
        I = i;
        J = j;
        K = k;
        Elements = elements;
    }
}
