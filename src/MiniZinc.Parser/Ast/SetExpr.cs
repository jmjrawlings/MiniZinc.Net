namespace MiniZinc.Parser.Ast;

public sealed record SetExpr : IExpr
{
    public List<IExpr> Elements { get; set; } = new();
}
