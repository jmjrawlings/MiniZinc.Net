namespace MiniZinc.Parser;

public sealed class Array1dCallExpr : MiniZincExpr
{
    public MiniZincExpr I { get; }
    public Array1dExpr Array { get; }

    public Array1dCallExpr(in Token start, MiniZincExpr i, Array1dExpr array)
        : base(start)
    {
        I = i;
        Array = array;
    }
}
