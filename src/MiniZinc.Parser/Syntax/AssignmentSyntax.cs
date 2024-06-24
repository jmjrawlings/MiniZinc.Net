namespace MiniZinc.Parser.Syntax;

public sealed record AssignmentSyntax(IdentifierSyntax Identifier, ExpressionSyntax Expr)
    : StatementSyntax(Identifier.Start),
        ILetLocalSyntax,
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;
}
