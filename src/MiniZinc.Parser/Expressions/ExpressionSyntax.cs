namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An expression lives inside statements and other expressions.
/// </summary>
public abstract class ExpressionSyntax : SyntaxNode
{
    /// <summary>
    /// An expression lives inside statements and other expressions.
    /// </summary>
    protected ExpressionSyntax(Token start)
        : base(start) { }
}
