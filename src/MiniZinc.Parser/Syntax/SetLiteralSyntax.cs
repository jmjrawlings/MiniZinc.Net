namespace MiniZinc.Parser.Syntax;

public sealed record SetLiteralSyntax(in Token Start) : ExpressionSyntax(Start)
{
    public List<SyntaxNode> Elements { get; } = new();
}
