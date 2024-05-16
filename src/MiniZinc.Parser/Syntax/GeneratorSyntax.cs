namespace MiniZinc.Parser.Syntax;

public sealed record GeneratorSyntax(Token Start) : SyntaxNode(Start)
{
    public required List<IdentifierSyntax> Names { get; set; }

    public required SyntaxNode From { get; set; }

    public SyntaxNode? Where { get; set; }
}
