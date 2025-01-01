namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An expression lives inside statements and other expressions.
/// </summary>
public abstract class Expr : Syntax
{
    /// <summary>
    /// An expression lives inside statements and other expressions.
    /// </summary>
    protected Expr(Token start)
        : base(start) { }
}
