namespace MiniZinc.Parser.Ast;

public sealed record AssignStatement(string name, SyntaxNode expr) : SyntaxNode, INamed, ILetLocal
{
    public string Name => name;

    public SyntaxNode Expr => expr;

    public override string ToString() => $"{name} = {expr}";
}
