namespace MiniZinc.Parser.Syntax;

public sealed class UnaryOperatorSyntax : ExpressionSyntax
{
    public Operator Operator { get; init; }

    public SyntaxNode Expr { get; init; }

    public UnaryOperatorSyntax(in Token start, Operator op, SyntaxNode expr)
        : base(start)
    {
        Operator = op;
        Expr = expr;
    }
}
