namespace MiniZinc.Parser.Ast;

public readonly struct Assignment(string name, INode expr) : INamed, INode, ILetLocal
{
    public string Name => name;

    public INode Expr => expr;

    public override string ToString() => $"{name} = {expr}";
}
