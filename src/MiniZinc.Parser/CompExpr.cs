namespace MiniZinc.Parser;

/// <summary>
/// An array or set comprehension
/// </summary>
public sealed class SetCompExpr : MiniZincExpr
{
    public MiniZincExpr Expr { get; }

    public IReadOnlyList<GenExpr> Generators { get; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public SetCompExpr(in Token start, MiniZincExpr expr, IReadOnlyList<GenExpr> generators)
        : base(start)
    {
        Expr = expr;
        Generators = generators;
    }
}

/// <summary>
/// An array or set comprehension
/// </summary>
public sealed class ArrayCompExpr : MiniZincExpr
{
    public MiniZincExpr Expr { get; }

    public IReadOnlyList<GenExpr> Generators { get; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public ArrayCompExpr(in Token start, MiniZincExpr expr, IReadOnlyList<GenExpr> generators)
        : base(start)
    {
        Expr = expr;
        Generators = generators;
    }
}
