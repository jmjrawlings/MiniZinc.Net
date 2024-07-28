namespace MiniZinc.Parser.Syntax;

public class BinaryOperatorSyntax : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public Token Infix { get; }
    public Operator? Operator { get; }
    public ExpressionSyntax Right { get; }

    public BinaryOperatorSyntax(
        ExpressionSyntax left,
        in Token infix,
        Operator? @operator,
        ExpressionSyntax right
    )
        : base(left.Start)
    {
        Left = left;
        Infix = infix;
        Operator = @operator;
        Right = right;
    }
}
