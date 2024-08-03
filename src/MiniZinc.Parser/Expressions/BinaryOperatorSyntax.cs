namespace MiniZinc.Parser.Syntax;

public class BinaryOperatorSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public Token Infix { get; }
    public TokenKind Operator { get; }
    public ExpressionSyntax Right { get; }

    public BinaryOperatorSyntax(ExpressionSyntax left, in Token op, ExpressionSyntax right)
        : base(left.Start)
    {
        Left = left;
        Infix = op;
        Operator = op.Kind;
        Right = right;
    }
}
