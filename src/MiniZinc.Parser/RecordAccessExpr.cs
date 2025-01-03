namespace MiniZinc.Parser;

public sealed class RecordAccessExpr : MiniZincExpr
{
    public readonly MiniZincExpr Expr;
    public readonly Token Field;

    public RecordAccessExpr(MiniZincExpr expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
