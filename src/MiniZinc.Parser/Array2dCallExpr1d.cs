namespace MiniZinc.Parser;

public sealed class Array2dCallExpr1d : MiniZincExpr
{
    public MiniZincExpr I { get; }

    public MiniZincExpr J { get; }

    public Array1dExpr Array { get; }

    public Array2dCallExpr1d(in Token start, MiniZincExpr i, MiniZincExpr j, Array1dExpr array)
        : base(start)
    {
        I = i;
        J = j;
        Array = array;
    }
}
