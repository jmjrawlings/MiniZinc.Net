namespace MiniZinc.Parser.Ast;

public sealed record ArrayExpr : IExpr
{
    public List<int> Dims { get; set; } = new();

    public int N => Dims.Count;

    public List<IExpr> Exprs { get; set; } = new();
}
