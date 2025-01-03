namespace MiniZinc.Parser;

/// <summary>
/// An array slice `..`
/// </summary>
/// <mzn>items[..]</mzn>
public sealed class SliceExpr : MiniZincExpr
{
    public SliceExpr(in Token start)
        : base(start) { }
}
