namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record ComprehensionSyntax(Token Start, SyntaxNode Expr) : SyntaxNode(Start)
{
    public required List<GeneratorSyntax> Generators { get; set; }
    public required bool IsSet { get; init; }
}
