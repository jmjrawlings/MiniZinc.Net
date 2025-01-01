namespace MiniZinc.Parser.Syntax;

public sealed class GenExpr : Expr
{
    public GenExpr(in Token start, List<Token> names)
        : base(start)
    {
        Names = names;
    }

    public List<Token> Names { get; }

    public required Expr From { get; set; }

    public Expr? Where { get; set; }
}
