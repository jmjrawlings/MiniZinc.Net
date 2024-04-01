namespace MiniZinc.Parser.Ast;

public sealed record Array1DLit : IExpr
{
    public int I { get; set; }
    public List<IExpr> Elements { get; set; } = new();
}
