namespace MiniZinc.Parser.Syntax;

public sealed class UnaryOperatorSyntax : ExpressionSyntax
{
    public Operator Operator { get; }

    public SyntaxNode Expr { get; }

    public UnaryOperatorSyntax(in Token start, Operator op, SyntaxNode expr)
        : base(start)
    {
        Operator = op;
        Expr = expr;
    }
}
