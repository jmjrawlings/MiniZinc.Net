namespace MiniZinc.Parser.Ast;

public sealed record ArrayExpr : IExpr
{
    public List<TypeInst> Dims { get; set; }

    public int N => Dims.Count;

    public List<IExpr> Exprs { get; set; } = new();
}
