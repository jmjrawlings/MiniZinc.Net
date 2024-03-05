namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : IExpr
{
    public List<(IExpr, IExpr)> Fields { get; set; } = new();
}
