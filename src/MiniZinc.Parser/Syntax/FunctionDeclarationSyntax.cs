namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
public sealed record FunctionDeclarationSyntax(Token Start, Token Name, TypeSyntax Type, List<ParameterSyntax>? Parameters, SyntaxNode? Body): SyntaxNode(Start)
{
    public Token? Ann { get; set; }
}