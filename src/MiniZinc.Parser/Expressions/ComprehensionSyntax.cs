namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An array or set comprehension
/// </summary>
public sealed class ComprehensionSyntax : ExpressionSyntax
{
    public ExpressionSyntax Expr { get; }

    public required List<GeneratorSyntax> Generators { get; set; }

    public required bool IsSet { get; init; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public ComprehensionSyntax(in Token start, ExpressionSyntax expr)
        : base(start)
    {
        Expr = expr;
    }
}
