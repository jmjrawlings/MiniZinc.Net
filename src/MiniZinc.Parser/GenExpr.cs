namespace MiniZinc.Parser;

public sealed class GenExpr : MiniZincExpr
{
    public GenExpr(in Token start, List<Token> names)
        : base(start)
    {
        Names = names;
    }

    public List<Token> Names { get; }

    public required MiniZincExpr From { get; set; }

    public MiniZincExpr? Where { get; set; }
}
