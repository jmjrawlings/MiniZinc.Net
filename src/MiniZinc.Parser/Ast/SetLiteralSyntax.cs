namespace MiniZinc.Parser.Ast;

public sealed record SetLiteralSyntax(in Token Start) : SyntaxNode(Start)
{
    public List<SyntaxNode> Elements { get; set; } = new();
}
