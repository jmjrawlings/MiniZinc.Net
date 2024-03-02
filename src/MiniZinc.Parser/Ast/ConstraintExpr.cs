namespace MiniZinc.Parser.Ast;

public sealed record ConstraintExpr(IExpr expr) : IExpr, IAnnotations
{
    public IExpr Expr { get; } = expr;
    public List<IExpr>? Annotations { get; set; }
}
