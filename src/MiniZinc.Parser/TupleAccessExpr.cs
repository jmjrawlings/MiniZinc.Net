namespace MiniZinc.Parser;

public sealed class TupleAccessExpr : MiniZincExpr
{
    public readonly MiniZincExpr Expr;

    public readonly Token Field;

    public int Index => Field.IntValue;

    public TupleAccessExpr(MiniZincExpr expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
