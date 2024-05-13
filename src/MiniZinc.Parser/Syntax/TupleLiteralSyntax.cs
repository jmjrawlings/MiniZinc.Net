namespace MiniZinc.Parser.Ast;

public sealed record TupleLiteralSyntax(Token Start) : SyntaxNode(Start)
{
    public List<SyntaxNode> Fields { get; set; } = new();
}
