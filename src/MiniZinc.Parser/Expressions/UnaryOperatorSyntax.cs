namespace MiniZinc.Parser.Syntax;

public sealed class UnaryOperatorSyntax : ExpressionSyntax
{
    public TokenKind Operator { get; }

    public ExpressionSyntax Expr { get; }

    public UnaryOperatorSyntax(in Token op, ExpressionSyntax expr)
        : base(op)
    {
        Operator = op.Kind;
        Expr = expr;
    }
}
