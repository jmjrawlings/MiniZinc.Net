namespace MiniZinc.Parser.Ast;

public sealed record ConstraintExpr : IExpr, IAnnotations
{
    public IExpr Expr { get; set; }
    public List<IExpr>? Annotations { get; set; }
}
