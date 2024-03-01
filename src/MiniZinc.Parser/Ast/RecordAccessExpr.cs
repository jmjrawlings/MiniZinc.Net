namespace MiniZinc.Parser.Ast;

public sealed record RecordAccessExpr : IExpr
{
    public IExpr Expr { get; set; }
    public string Field { get; set; }
}