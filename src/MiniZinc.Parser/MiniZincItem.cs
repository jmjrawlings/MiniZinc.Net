namespace MiniZinc.Parser;

/// <summary>
/// A statement lives at the top level of a MiniZinc model as
/// opposed to expressions.
/// </summary>
public abstract class MiniZincItem : MiniZincSyntax
{
    /// <summary>
    /// A statement lives at the top level of a MiniZinc model as
    /// opposed to expressions.
    /// </summary>
    protected MiniZincItem(in Token start)
        : base(start) { }
}
