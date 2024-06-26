namespace MiniZinc.Parser.Syntax;

public sealed class TypeAliasSyntax : StatementSyntax, IIdentifiedSyntax
{
    public TypeAliasSyntax(in Token start, IdentifierSyntax identifier, TypeSyntax type)
        : base(in start)
    {
        Identifier = identifier;
        Type = type;
    }

    public string Name => Identifier.Name;

    public readonly IdentifierSyntax Identifier;

    public readonly TypeSyntax Type;

    IdentifierSyntax IIdentifiedSyntax.Identifier => Identifier;
}
