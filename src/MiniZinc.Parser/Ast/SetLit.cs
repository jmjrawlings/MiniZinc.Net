namespace MiniZinc.Parser.Ast;

public sealed record SetLit : IExpr
{
    public List<IExpr> Elements { get; set; } = new();
}
