namespace MiniZinc.Parser.Syntax;

public record ArraySyntax(in Token Start) : ExpressionSyntax(Start)
{
    public List<SyntaxNode> Elements { get; } = new();
    public int N => Elements.Count;
}
