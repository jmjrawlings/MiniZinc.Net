namespace MiniZinc.Parser.Ast;

public sealed record AssignStatement(string name, Node expr) : Node, INamed, ILetLocal
{
    public string Name => name;

    public Node Expr => expr;

    public override string ToString() => $"{name} = {expr}";
}
