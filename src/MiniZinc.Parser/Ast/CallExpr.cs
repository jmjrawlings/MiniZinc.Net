namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : IExpr, INamed
{
    public string Name { get; set; }
    public List<IExpr> Args { get; set; } = new();
}
