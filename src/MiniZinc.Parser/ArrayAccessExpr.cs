namespace MiniZinc.Parser;

public sealed class ArrayAccessExpr : MiniZincExpr
{
    public MiniZincExpr Array { get; }
    public List<MiniZincExpr> Access { get; }

    public ArrayAccessExpr(MiniZincExpr array, List<MiniZincExpr> access)
        : base(array.Start)
    {
        Array = array;
        Access = access;
    }
}
