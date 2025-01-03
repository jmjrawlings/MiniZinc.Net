namespace MiniZinc.Parser;

/// <summary>
/// An array or set comprehension
/// </summary>
public sealed class CompExpr : MiniZincExpr
{
    public MiniZincExpr Expr { get; }

    public required List<GenExpr> Generators { get; set; }

    public required bool IsSet { get; init; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public CompExpr(in Token start, MiniZincExpr expr)
        : base(start)
    {
        Expr = expr;
    }
}
