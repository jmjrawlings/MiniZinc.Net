namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem : IExpr, IAnnotations
{
    public IExpr Expr;
    public List<IExpr>? Annotations { get; set; }
}
