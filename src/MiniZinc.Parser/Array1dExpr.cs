namespace MiniZinc.Parser;

public sealed class Array1dExpr : ArrayExpr
{
    public Array1dExpr(in Token start, List<MiniZincExpr>? elements)
        : base(start, elements) { }

    public bool Indexed { get; set; }
}
