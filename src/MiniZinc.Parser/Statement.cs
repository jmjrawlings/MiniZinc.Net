namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A statement lives at the top level of a MiniZinc model as
/// opposed to expressions.
/// </summary>
public abstract class Statement : Syntax
{
    /// <summary>
    /// A statement lives at the top level of a MiniZinc model as
    /// opposed to expressions.
    /// </summary>
    protected Statement(in Token start)
        : base(start) { }
}
