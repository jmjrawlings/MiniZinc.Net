namespace MiniZinc.Parser.Ast;

public sealed record VariableAssignmentSyntax(in Token Name, SyntaxNode Expr)
    : SyntaxNode(Name),
        ILetLocal
{
    public override string ToString() => $"{Name} = {Expr}";
}
