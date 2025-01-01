namespace MiniZinc.Parser.Syntax;

public sealed class RecordAccessExpr : Expr
{
    public readonly Expr Expr;
    public readonly Token Field;

    public RecordAccessExpr(Expr expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
