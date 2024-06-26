namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An array or set comprehension
/// </summary>
public class ComprehensionSyntax : ExpressionSyntax
{
    public SyntaxNode Expr { get; }

    public required List<GeneratorSyntax> Generators { get; set; }

    public required bool IsSet { get; init; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public ComprehensionSyntax(in Token start, SyntaxNode expr)
        : base(start)
    {
        Expr = expr;
    }
}
