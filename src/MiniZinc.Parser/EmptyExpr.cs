namespace MiniZinc.Parser;

using Syntax;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = &lt;&gt;>;</mzn>
public sealed class EmptyExpr : Expr
{
    /// <summary>
    /// An empty value used for optional types
    /// </summary>
    /// <mzn>opt int: x = &lt;&gt;>;</mzn>
    public EmptyExpr(in Token start)
        : base(start) { }
}
