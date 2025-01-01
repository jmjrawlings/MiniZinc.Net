namespace MiniZinc.Parser.Syntax;

public sealed class RecordExpr : Expr
{
    public RecordExpr(in Token start)
        : base(start) { }

    public List<(Token, Expr)> Fields { get; } = [];
}
