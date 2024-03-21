namespace MiniZinc.Parser.Ast;

public sealed record SetLit : INode
{
    public List<INode> Elements { get; set; } = new();
}
