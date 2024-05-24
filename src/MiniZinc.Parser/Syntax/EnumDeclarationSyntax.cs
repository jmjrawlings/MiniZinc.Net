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
    public IdentifierSyntax? Constructor { get; set; } = null;
    public List<IdentifierSyntax>? Names { get; set; } = null;
}

public sealed record EnumDeclarationSyntax(in Token Start, IdentifierSyntax Name)
    : SyntaxNode(Start)
{
    public List<EnumCasesSyntax> Cases { get; } = new();
}
