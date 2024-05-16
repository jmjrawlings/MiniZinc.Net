namespace MiniZinc.Parser.Syntax;

public abstract record SyntaxNode(in Token Start)
{
    public List<SyntaxNode>? Annotations { get; set; } = null;
}