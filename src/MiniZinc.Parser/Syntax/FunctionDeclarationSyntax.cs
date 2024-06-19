namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
public sealed record FunctionDeclarationSyntax(
    in Token Start,
    IdentifierSyntax Identifier,
    TypeSyntax Type,
    List<ParameterSyntax>? Parameters,
    SyntaxNode? Body
) : SyntaxNode(Start), IIdentifiedSyntax
{
    public string Name => Identifier.Name;
    public IdentifierSyntax? Ann { get; set; }
}
