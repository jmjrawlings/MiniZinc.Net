namespace MiniZinc.Parser.Syntax;

public sealed record ArrayAccessSyntax(SyntaxNode Array, List<SyntaxNode> Access)
    : SyntaxNode(Array.Start)
{
    public override string ToString() => $"{Array.Write()}[{Access.Write()}]";
}
