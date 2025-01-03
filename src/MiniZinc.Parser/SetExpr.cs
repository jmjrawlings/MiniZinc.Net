namespace MiniZinc.Parser;

public sealed class SetExpr : MiniZincExpr
{
    public IReadOnlyList<MiniZincExpr>? Elements { get; }

    public SetExpr(in Token start, IReadOnlyList<MiniZincExpr>? elements)
        : base(start)
    {
        Elements = elements;
    }
}
