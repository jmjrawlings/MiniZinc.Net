namespace MiniZinc.Parser.Ast;

public sealed record Array2DLit : INode
{
    public int I { get; set; }
    public int J { get; set; }
    public List<INode> Elements { get; set; } = new();
}
