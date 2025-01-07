namespace MiniZinc.Parser;

/// <summary>
///
/// </summary>
/// <mzn>forall(x in [1,2,3] where x > 1)(xs[x])</mzn>
public sealed class GenCallExpr : MiniZincExpr
{
    public Token Name { get; }
    public MiniZincExpr Expr { get; }
    public IReadOnlyList<GenExpr> Generators { get; }

    public GenCallExpr(Token name, MiniZincExpr expr, IReadOnlyList<GenExpr> generators)
        : base(name)
    {
        Name = name;
        Expr = expr;
        Generators = generators;
    }
}
