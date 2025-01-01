namespace MiniZinc.Parser.Syntax;

public sealed class ConstraintStatement : Statement, ILetLocalSyntax
{
    public ConstraintStatement(in Token start, Expr expr)
        : base(start)
    {
        Expr = expr;
    }

    public readonly Expr Expr;
}
