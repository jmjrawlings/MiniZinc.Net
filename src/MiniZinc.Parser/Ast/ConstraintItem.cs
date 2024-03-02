namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem(IExpr expr) : IExpr, IAnnotations
{
    public IExpr Expr { get; } = expr;
    public List<IExpr>? Annotations { get; set; }
}
