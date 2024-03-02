namespace MiniZinc.Parser.Ast;

public sealed record OutputItem : IStatement, IAnnotations
{
    public IExpr Expr { get; set; }
    public List<IExpr>? Annotations { get; set; }
}
