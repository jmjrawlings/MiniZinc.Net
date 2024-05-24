namespace MiniZinc.Parser.Syntax;

public sealed record TupleLiteralSyntax(in Token Start) : SyntaxNode(Start)
{
    public List<SyntaxNode> Fields { get; set; } = new();
}
