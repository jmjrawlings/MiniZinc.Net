namespace MiniZinc.Parser;

public sealed class RecordExpr : MiniZincExpr
{
    public IReadOnlyList<(IdentExpr, MiniZincExpr)> Fields;

    public RecordExpr(in Token start, IReadOnlyList<(IdentExpr, MiniZincExpr)> fields)
        : base(start)
    {
        Fields = fields;
    }
}
