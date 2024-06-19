namespace MiniZinc.Parser.Syntax;

public sealed record AssignmentSyntax(IdentifierSyntax Identifier, SyntaxNode Expr)
    : SyntaxNode(Identifier.Start),
        ILetLocalSyntax,
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;
}
