namespace MiniZinc.Parser.Ast;

public sealed record OutputItem : IStatement
{
    public IExpr Expr { get; set; }
    public IExpr? Annotation { get; set; }
}
