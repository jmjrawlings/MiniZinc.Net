namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
public sealed record FunctionDeclarationSyntax(
    in Token Start,
    IdentifierSyntax Name,
    TypeSyntax Type,
    List<ParameterSyntax>? Parameters,
    SyntaxNode? Body
) : SyntaxNode(Start)
{
    public IdentifierSyntax? Ann { get; set; }
}
