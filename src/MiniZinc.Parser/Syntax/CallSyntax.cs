namespace MiniZinc.Parser.Syntax;

public sealed record CallSyntax(IdentifierSyntax Name) : ExpressionSyntax(Name.Start)
{
    public List<SyntaxNode>? Args { get; init; }
}
