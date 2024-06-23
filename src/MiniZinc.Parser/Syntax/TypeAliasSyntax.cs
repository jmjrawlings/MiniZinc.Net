namespace MiniZinc.Parser.Syntax;

public sealed record TypeAliasSyntax(in Token Start, IdentifierSyntax Identifier, TypeSyntax Type)
    : StatementSyntax(in Start),
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;
}
