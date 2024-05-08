namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record VariableDeclarationSyntax(Token Start) : SyntaxNode(Start), ILetLocal
{
    public Token Name { get; set; }

    public required TypeInstSyntax Type { get; set; }

    public SyntaxNode? Body { get; set; }

    public List<(Token, TypeInstSyntax)>? Parameters { get; set; }

    public bool IsFunction { get; set; }

    public bool IsAnnotation { get; set; }
}
