namespace MiniZinc.Parser;

public class ArrayExpr : MiniZincExpr
{
    public ArrayExpr(in Token start, IReadOnlyList<MiniZincExpr>? elements)
        : base(start)
    {
        Elements = elements;
    }

    public IReadOnlyList<MiniZincExpr>? Elements { get; }
    public int N => Elements?.Count ?? 0;
}
