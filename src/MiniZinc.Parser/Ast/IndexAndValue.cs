namespace MiniZinc.Parser.Ast;

public readonly record struct IndexAndValue(INode index, INode value) : INode
{
    public INode Index => index;
    public INode Value => value;
}
