namespace MiniZinc.Parser.Syntax;

public sealed record CallSyntax(Token Name) : SyntaxNode(Name)
{
    public List<SyntaxNode>? Args { get; set; }
}
