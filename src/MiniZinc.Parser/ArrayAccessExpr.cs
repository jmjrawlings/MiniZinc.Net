namespace MiniZinc.Parser;

public sealed class ArrayAccessExpr : MiniZincExpr
{
    public MiniZincExpr Array { get; }

    public IReadOnlyList<MiniZincExpr> Access { get; }

    public ArrayAccessExpr(MiniZincExpr array, IReadOnlyList<MiniZincExpr> access)
        : base(array.Start)
    {
        Array = array;
        Access = access;
    }
}
