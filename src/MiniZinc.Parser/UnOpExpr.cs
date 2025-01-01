namespace MiniZinc.Parser.Syntax;

public sealed class UnOpExpr : Expr
{
    public TokenKind Operator { get; }

    public Expr Expr { get; }

    public UnOpExpr(in Token op, Expr expr)
        : base(op)
    {
        Operator = op.Kind;
        Expr = expr;
    }
}
