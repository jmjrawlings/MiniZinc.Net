namespace MiniZinc.Parser;

public class BinOpExpr : MiniZincExpr
{
    public MiniZincExpr Left { get; }
    public Token Infix { get; }
    public TokenKind Operator { get; }
    public MiniZincExpr Right { get; }

    public BinOpExpr(MiniZincExpr left, in Token infix, MiniZincExpr right)
        : base(left.Start)
    {
        Left = left;
        Infix = infix;
        Operator = infix.Kind;
        Right = right;
    }

    public BinOpExpr(MiniZincExpr left, TokenKind op, MiniZincExpr right)
        : base(left.Start)
    {
        Left = left;
        Infix = default;
        Operator = op;
        Right = right;
    }
}
