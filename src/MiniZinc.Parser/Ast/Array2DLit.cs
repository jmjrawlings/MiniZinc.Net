namespace MiniZinc.Parser.Ast;

public sealed record Array2DLit : IExpr
{
    public List<IExpr> Indices { get; set; } = new();

    public List<IExpr> Elements { get; set; } = new();

    public int Rows { get; set; }

    public int Cols { get; set; }

    public bool RowIndexed { get; set; }

    public bool ColIndexed { get; set; }
}
