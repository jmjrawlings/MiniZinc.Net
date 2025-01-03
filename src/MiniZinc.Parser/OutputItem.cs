namespace MiniZinc.Parser;

public sealed class OutputItem : MiniZincItem
{
    public readonly MiniZincExpr Expr;

    public OutputItem(in Token start, MiniZincExpr expr)
        : base(start)
    {
        Expr = expr;
    }
}
