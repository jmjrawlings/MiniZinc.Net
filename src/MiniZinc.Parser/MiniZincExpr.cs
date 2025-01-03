namespace MiniZinc.Parser;

/// <summary>
/// A MiniZinc expression
/// </summary>
/// <mzn>1</mzn>
/// <mzn>a + b</mzn>
/// <mzn>[1, -1, 2, 3 + 5]</mzn>
public abstract class MiniZincExpr : MiniZincSyntax
{
    /// <summary>
    /// An expression lives inside statements and other expressions.
    /// </summary>
    protected MiniZincExpr(Token start)
        : base(start) { }
}
