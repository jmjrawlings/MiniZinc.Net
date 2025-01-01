namespace MiniZinc.Parser.Syntax;

/// <summary>
/// An array or set comprehension
/// </summary>
public sealed class CompExpr : Expr
{
    public Expr Expr { get; }

    public required List<GenExpr> Generators { get; set; }

    public required bool IsSet { get; init; }

    /// <summary>
    /// An array or set comprehension
    /// </summary>
    public CompExpr(in Token start, Expr expr)
        : base(start)
    {
        Expr = expr;
    }
}
