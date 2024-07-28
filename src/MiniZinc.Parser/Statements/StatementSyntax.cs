namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A statement lives at the top level of a MiniZinc model as
/// opposed to expressions.
/// </summary>
public abstract class StatementSyntax : SyntaxNode
{
    /// <summary>
    /// A statement lives at the top level of a MiniZinc model as
    /// opposed to expressions.
    /// </summary>
    protected StatementSyntax(in Token start)
        : base(start) { }
}
