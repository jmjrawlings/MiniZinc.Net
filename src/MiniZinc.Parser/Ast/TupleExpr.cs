namespace MiniZinc.Parser.Ast;

public sealed record TupleExpr : IExpr
{
    public List<IExpr> Exprs { get; set; } = new();
}