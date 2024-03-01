namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : IExpr
{
    public Dictionary<string, IExpr> Exprs { get; set; } = new();
}