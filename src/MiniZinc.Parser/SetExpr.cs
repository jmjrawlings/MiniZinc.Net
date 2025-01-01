namespace MiniZinc.Parser.Syntax;

public sealed class SetExpr : Expr
{
    public IReadOnlyList<Expr>? Elements { get; }

    public SetExpr(in Token start, IReadOnlyList<Expr>? elements = null)
        : base(start)
    {
        Elements = elements;
    }
}
