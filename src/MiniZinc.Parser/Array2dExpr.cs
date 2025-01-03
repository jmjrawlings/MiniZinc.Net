namespace MiniZinc.Parser;

public sealed class Array2dExpr : ArrayExpr
{
    public Array2dExpr(
        in Token start,
        IReadOnlyList<MiniZincExpr>? elements,
        IReadOnlyList<MiniZincExpr>? indices,
        int i,
        int j,
        bool rowIndexed,
        bool colIndexed
    )
        : base(start, elements)
    {
        Indices = indices;
        I = i;
        J = j;
        RowIndexed = rowIndexed;
        ColIndexed = colIndexed;
    }

    public IReadOnlyList<MiniZincSyntax>? Indices { get; }

    public int I { get; }

    public int J { get; }

    public bool RowIndexed { get; }

    public bool ColIndexed { get; }
}
