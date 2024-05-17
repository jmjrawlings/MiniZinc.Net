namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A variable
/// </summary>
public sealed record DeclarationSyntax(Token Start, TypeSyntax Type) : SyntaxNode(Start), ILetLocal
{
    public IdentifierSyntax Name { get; set; }
    
    public SyntaxNode? Body { get; set; }
    
    public List<ParameterSyntax>? Parameters { get; set; }
}