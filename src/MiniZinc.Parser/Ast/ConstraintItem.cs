namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem : IExpr, IAnnotations
{
    public IExpr Expr { get; set; }
    public List<IExpr>? Annotations { get; set; }
}
