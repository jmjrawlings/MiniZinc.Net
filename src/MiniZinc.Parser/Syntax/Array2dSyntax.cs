namespace MiniZinc.Parser.Syntax;

public sealed record Array2dSyntax(in Token Start) : ArraySyntax(Start)
{
    public List<SyntaxNode> Indices { get; set; } = new();

    public int I { get; set; }

    public int J { get; set; }

    public bool RowIndexed { get; set; }

    public bool ColIndexed { get; set; }

    public override string ToString() => $"<{I}x{J} array of {N} items>";
}
