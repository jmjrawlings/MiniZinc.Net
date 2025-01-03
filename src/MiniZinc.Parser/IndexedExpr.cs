namespace MiniZinc.Parser;

public sealed class IndexedExpr : MiniZincExpr
{
    public readonly MiniZincExpr Index;
    public readonly MiniZincExpr Value;

    public IndexedExpr(MiniZincExpr index, MiniZincExpr value)
        : base(index.Start)
    {
        Index = index;
        Value = value;
    }
}
