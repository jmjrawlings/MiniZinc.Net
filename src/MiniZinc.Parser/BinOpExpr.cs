namespace MiniZinc.Parser.Syntax;

public class BinOpExpr : Expr
{
    public Expr Left { get; }
    public Token Infix { get; }
    public TokenKind Operator { get; }
    public Expr Right { get; }

    public BinOpExpr(Expr left, in Token infix, Expr right)
        : base(left.Start)
    {
        Left = left;
        Infix = infix;
        Operator = infix.Kind;
        Right = right;
    }
}
