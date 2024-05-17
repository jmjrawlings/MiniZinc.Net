namespace MiniZinc.Parser.Syntax;

public sealed record CallSyntax(IdentifierSyntax Name) : SyntaxNode(Name.Start)
{
    public List<SyntaxNode>? Args { get; set; }
}
