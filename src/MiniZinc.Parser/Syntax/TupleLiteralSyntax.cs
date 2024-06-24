namespace MiniZinc.Parser.Syntax;

public sealed record TupleLiteralSyntax(in Token Start) : ExpressionSyntax(Start)
{
    public List<SyntaxNode> Fields { get; set; } = new();
}
