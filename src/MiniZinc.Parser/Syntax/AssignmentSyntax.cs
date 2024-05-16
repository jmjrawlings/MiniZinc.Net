namespace MiniZinc.Parser.Syntax;

public sealed record AssignmentSyntax(in Token Name, SyntaxNode Expr) : SyntaxNode(Name), ILetLocal
{
    public override string ToString() => $"{Name} = {Expr}";
}
