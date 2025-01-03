namespace MiniZinc.Parser;

public sealed class TupleExpr : MiniZincExpr
{
    public readonly List<MiniZincExpr> Fields;

    public TupleExpr(in Token start)
        : base(start)
    {
        Fields = [];
    }
}
