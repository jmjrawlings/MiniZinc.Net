namespace MiniZinc.Parser.Syntax;

public sealed class TupleExpr : Expr
{
    public readonly List<Expr> Fields;

    public TupleExpr(in Token start)
        : base(start)
    {
        Fields = [];
    }
}
