namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
public sealed record FunctionDeclarationSyntax(Token Start, IdentifierSyntax Name, TypeSyntax Type, List<ParameterSyntax>? Parameters, SyntaxNode? Body): SyntaxNode(Start)
{
    public IdentifierSyntax? Ann { get; set; }
}