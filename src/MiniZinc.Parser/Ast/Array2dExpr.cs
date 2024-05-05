namespace MiniZinc.Parser.Ast;

public sealed record Array2dExpr(Token start) : ArrayExpr(start)
{
    public List<SyntaxNode> Indices { get; set; } = new();

    public int I { get; set; }

    public int J { get; set; }

    public bool RowIndexed { get; set; }

    public bool ColIndexed { get; set; }

    public override string ToString() => $"<{I}x{J} array of {N} items>";
}
