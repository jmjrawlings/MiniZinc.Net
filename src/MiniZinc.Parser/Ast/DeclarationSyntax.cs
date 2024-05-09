namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record DeclarationSyntax(Token Start, TypeSyntax Type) : SyntaxNode(Start), ILetLocal
{
    public Token Name { get; set; }

    public SyntaxNode? Body { get; set; }

    public List<(Token, TypeSyntax)>? Parameters { get; set; }

    public bool IsFunction { get; set; }

    public bool IsAnnotation { get; set; }
}
