namespace MiniZinc.Parser.Syntax;

public sealed class UnaryOperatorSyntax : ExpressionSyntax
{
    public Operator Operator { get; }

    public ExpressionSyntax Expr { get; }

    public UnaryOperatorSyntax(in Token start, Operator op, ExpressionSyntax expr)
        : base(start)
    {
        Operator = op;
        Expr = expr;
    }
}
