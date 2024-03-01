namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : IExpr
{
    public string Name { get; set; }
    public List<IExpr> Args { get; set; } = new();
}
