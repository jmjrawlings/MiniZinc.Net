namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed record SyntaxTree(in Token Start) : SyntaxNode(Start)
{
    public readonly List<SyntaxNode> Nodes = new();
}
