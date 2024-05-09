namespace MiniZinc.Parser.Ast;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed record SyntaxTree(Token Start) : SyntaxNode(Start)
{
    public readonly List<SyntaxNode> Nodes = new();
}
