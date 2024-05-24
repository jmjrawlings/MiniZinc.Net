namespace MiniZinc.Parser.Syntax;

public sealed record SetLiteralSyntax(in Token Start) : SyntaxNode(Start)
{
    public List<SyntaxNode> Elements { get; } = new();
}
