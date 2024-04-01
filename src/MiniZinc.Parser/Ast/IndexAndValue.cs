namespace MiniZinc.Parser.Ast;

public readonly record struct IndexAndValue(IExpr index, IExpr value) : IExpr
{
    public INode Index => index;
    public INode Value => value;
}
