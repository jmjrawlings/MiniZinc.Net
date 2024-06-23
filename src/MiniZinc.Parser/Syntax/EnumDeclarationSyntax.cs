namespace MiniZinc.Parser.Syntax;

public enum EnumCaseType
{
    Names,
    Complex,
    Underscore,
    Anon
}

public record EnumCasesSyntax(in Token Start, EnumCaseType Type, SyntaxNode? Expr = null)
    : SyntaxNode(Start)
{
    public IdentifierSyntax? Constructor { get; init; } = null;
    public List<IdentifierSyntax>? Names { get; init; } = null;
}

public sealed record EnumDeclarationSyntax(in Token Start, IdentifierSyntax Identifier)
    : StatementSyntax(Start),
        IIdentifiedSyntax
{
    public string Name => Identifier.Name;
    public List<EnumCasesSyntax> Cases { get; } = new();
}
