namespace MiniZinc.Parser.Syntax;

public sealed class ConstraintSyntax : StatementSyntax, ILetLocalSyntax
{
    public ConstraintSyntax(in Token start, ExpressionSyntax expr)
        : base(start)
    {
        Expr = expr;
    }

    public readonly ExpressionSyntax Expr;
}
