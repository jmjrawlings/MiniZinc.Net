namespace MiniZinc.Parser;

public sealed class TupleExpr : MiniZincExpr
{
    public IReadOnlyList<MiniZincExpr> Fields { get; }

    public TupleExpr(in Token start, IReadOnlyList<MiniZincExpr> fields)
        : base(start)
    {
        Fields = fields;
    }
}
