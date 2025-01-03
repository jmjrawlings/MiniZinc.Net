namespace MiniZinc.Parser;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = &lt;&gt;>;</mzn>
public sealed class EmptyExpr : MiniZincExpr
{
    /// <summary>
    /// An empty value used for optional types
    /// </summary>
    /// <mzn>opt int: x = &lt;&gt;>;</mzn>
    public EmptyExpr(in Token start)
        : base(start) { }
}
