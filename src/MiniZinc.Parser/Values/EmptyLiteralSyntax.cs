using MiniZinc.Parser.Syntax;

namespace MiniZinc.Parser;

/// <summary>
/// An empty value used for optional types
/// </summary>
/// <mzn>opt int: x = &lt;&gt;>;</mzn>
public sealed class EmptyLiteralSyntax : ValueSyntax
{
    /// <summary>
    /// An empty value used for optional types
    /// </summary>
    /// <mzn>opt int: x = &lt;&gt;>;</mzn>
    public EmptyLiteralSyntax(in Token start)
        : base(start) { }
}
