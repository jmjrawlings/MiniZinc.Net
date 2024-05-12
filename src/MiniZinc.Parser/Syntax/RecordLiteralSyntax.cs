namespace MiniZinc.Parser.Ast;

public sealed record RecordLiteralSyntax(Token Start) : SyntaxNode(Start)
{
    public List<(Token, SyntaxNode)> Fields { get; } = new();
}
