namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem : IExpr, IAnnotations, IItem
{
    public IExpr Expr { get; set; }
    public List<IExpr>? Annotations { get; set; }
}
