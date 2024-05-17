namespace MiniZinc.Parser.Syntax;

public sealed record RecordLiteralSyntax(Token Start) : SyntaxNode(Start)
{
    public List<(IdentifierSyntax, SyntaxNode)> Fields { get; } = new();
}