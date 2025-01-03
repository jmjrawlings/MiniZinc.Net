namespace MiniZinc.Parser;

public sealed class ConstraintItem : MiniZincItem, ILetLocalSyntax
{
    public MiniZincExpr Expr { get; }

    public ConstraintItem(in Token start, MiniZincExpr expr)
        : base(start)
    {
        Expr = expr;
    }
}
