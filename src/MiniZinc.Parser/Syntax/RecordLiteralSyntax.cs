namespace MiniZinc.Parser.Syntax;

public sealed record RecordLiteralSyntax(in Token Start) : ExpressionSyntax(Start)
{
    public List<(IdentifierSyntax, SyntaxNode)> Fields { get; } = new();
}
