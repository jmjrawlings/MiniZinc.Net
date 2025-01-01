namespace MiniZinc.Parser.Syntax;

public class ArrayExpr : Expr
{
    public ArrayExpr(in Token start)
        : base(start) { }

    public List<Expr> Elements { get; } = [];
    public int N => Elements.Count;
}
