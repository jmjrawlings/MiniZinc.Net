namespace MiniZinc.Parser.Ast;

public sealed record OutputItem : IAnnotations
{
    public IExpr Expr { get; set; }
    public List<IExpr>? Annotations { get; set; }
}
