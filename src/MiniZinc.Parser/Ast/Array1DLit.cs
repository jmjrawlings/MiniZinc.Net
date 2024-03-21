namespace MiniZinc.Parser.Ast;

public sealed record Array1DLit : INode
{
    public int I { get; set; }
    public List<INode> Elements { get; set; } = new();
}
