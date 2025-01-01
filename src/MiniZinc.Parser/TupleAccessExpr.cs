namespace MiniZinc.Parser.Syntax;

public sealed class TupleAccessExpr : Expr
{
    public readonly Expr Expr;

    public readonly Token Field;

    public int Index => Field.IntValue;

    public TupleAccessExpr(Expr expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
