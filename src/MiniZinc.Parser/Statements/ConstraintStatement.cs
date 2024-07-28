namespace MiniZinc.Parser.Syntax;

public sealed class ConstraintStatement : StatementSyntax, ILetLocalSyntax
{
    public ConstraintStatement(in Token start, ExpressionSyntax expr)
        : base(start)
    {
        Expr = expr;
    }

    public readonly ExpressionSyntax Expr;
}
