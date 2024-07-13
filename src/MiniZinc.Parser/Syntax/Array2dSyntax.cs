namespace MiniZinc.Parser.Syntax;

public sealed class Array2dSyntax : ArraySyntax
{
    public Array2dSyntax(in Token start)
        : base(start) { }

    public List<SyntaxNode> Indices { get; set; } = new();

    public int I { get; set; }

    public int J { get; set; }

    public bool RowIndexed { get; set; }

    public bool ColIndexed { get; set; }
}
