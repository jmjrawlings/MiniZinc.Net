namespace MiniZinc.Parser.Ast;

public sealed record OutputExpr : IExpr
{
    public IExpr Expr { get; set; }
    public IExpr? Annotation { get; set; }
}