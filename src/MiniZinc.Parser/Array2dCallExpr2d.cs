namespace MiniZinc.Parser;

public sealed class Array2dCallExpr2d : MiniZincExpr
{
    public MiniZincExpr I { get; }

    public MiniZincExpr J { get; }

    public Array2dExpr Array { get; }

    public Array2dCallExpr2d(in Token start, MiniZincExpr i, MiniZincExpr j, Array2dExpr array)
        : base(start)
    {
        I = i;
        J = j;
        Array = array;
    }
}
