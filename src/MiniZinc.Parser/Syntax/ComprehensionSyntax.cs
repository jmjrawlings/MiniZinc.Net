namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An array or set comprehension
/// </summary>
public record ComprehensionSyntax(in Token Start, SyntaxNode Expr) : ExpressionSyntax(Start)
{
    public required List<GeneratorSyntax> Generators { get; set; }
    public required bool IsSet { get; init; }
}
