namespace MiniZinc.Parser.Syntax;

public sealed class Array2dExpr : ArrayExpr
{
    public Array2dExpr(in Token start)
        : base(start) { }

    public List<Syntax> Indices { get; set; } = [];

    public int I { get; set; }

    public int J { get; set; }

    public bool RowIndexed { get; set; }

    public bool ColIndexed { get; set; }
}
