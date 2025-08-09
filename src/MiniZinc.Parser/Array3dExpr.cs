namespace MiniZinc.Parser;

public sealed class Array3dExpr : ArrayExpr
{
    public Array3dExpr(in Token start, List<MiniZincExpr>? elements)
        : base(start, elements) { }

    public int I { get; set; }
    public int J { get; set; }
    public int K { get; set; }

    public override string ToString() => $"<{I}x{J}x{K} array of {N} items>";
}
