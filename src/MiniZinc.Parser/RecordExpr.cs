namespace MiniZinc.Parser;

public sealed class RecordExpr : MiniZincExpr
{
    public RecordExpr(in Token start)
        : base(start) { }

    public List<(Token, MiniZincExpr)> Fields { get; } = [];
}
