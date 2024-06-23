namespace MiniZinc.Parser.Syntax;

public sealed record AssignmentSyntax(IdentifierSyntax Identifier, SyntaxNode Expr)
    : StatementSyntax(Identifier.Start),
        ILetLocalSyntax,
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;
}
