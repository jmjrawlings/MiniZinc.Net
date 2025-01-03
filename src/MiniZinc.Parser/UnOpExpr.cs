namespace MiniZinc.Parser;

public sealed class UnOpExpr : MiniZincExpr
{
    public TokenKind Operator { get; }

    public MiniZincExpr Expr { get; }

    public UnOpExpr(in Token op, MiniZincExpr expr)
        : base(op)
    {
        Operator = op.Kind;
        Expr = expr;
    }
}
