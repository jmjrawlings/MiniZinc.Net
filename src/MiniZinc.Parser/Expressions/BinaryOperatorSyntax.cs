namespace MiniZinc.Parser.Syntax;

public class BinaryOperatorSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public Token Infix { get; }
    public TokenKind Operator { get; }
    public ExpressionSyntax Right { get; }

    public BinaryOperatorSyntax(ExpressionSyntax left, in Token infix, ExpressionSyntax right)
        : base(left.Start)
    {
        Left = left;
        Infix = infix;
        Operator = infix.Kind;
        Right = right;
    }
}
