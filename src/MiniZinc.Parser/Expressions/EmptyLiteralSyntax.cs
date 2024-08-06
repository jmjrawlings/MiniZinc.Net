namespace MiniZinc.Parser;

using Syntax;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = &lt;&gt;>;</mzn>
public sealed class EmptyLiteralSyntax : ExpressionSyntax
{
    /// <summary>
    /// An empty value used for optional types
    /// </summary>
    /// <mzn>opt int: x = &lt;&gt;>;</mzn>
    public EmptyLiteralSyntax(in Token start)
        : base(start) { }
}
