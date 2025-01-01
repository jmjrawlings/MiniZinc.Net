namespace MiniZinc.Parser.Syntax;

public sealed class IndexedExpr : Expr
{
    public readonly Expr Index;
    public readonly Expr Value;

    public IndexedExpr(Expr index, Expr value)
        : base(index.Start)
    {
        Index = index;
        Value = value;
    }
}
