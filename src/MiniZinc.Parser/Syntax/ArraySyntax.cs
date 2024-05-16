namespace MiniZinc.Parser.Syntax;

public record ArraySyntax(Token Start) : SyntaxNode(Start)
{
    public List<SyntaxNode> Elements { get; set; } = new();
    public int N => Elements.Count;
}
