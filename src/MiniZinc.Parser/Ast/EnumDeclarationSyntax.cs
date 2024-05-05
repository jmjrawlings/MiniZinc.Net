namespace MiniZinc.Parser.Ast;

public enum EnumCaseType
{
    Names,
    Complex,
    Underscore,
    Anon
}

public record EnumCasesSyntax(Token Start, EnumCaseType Type, SyntaxNode? Expr = null)
    : SyntaxNode(Start)
{
    public List<Token>? Names { get; set; } = null;
}

public sealed record EnumDeclarationSyntax(Token Start, Token Name) : SyntaxNode(Start)
{
    public List<EnumCasesSyntax> Cases { get; } = new();
}
