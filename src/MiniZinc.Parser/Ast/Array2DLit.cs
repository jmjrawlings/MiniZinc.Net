namespace MiniZinc.Parser.Ast;

public sealed record Array2DLit : IExpr
{
    public int I { get; set; }
    public int J { get; set; }
    public List<IExpr> Elements { get; set; } = new();
}
