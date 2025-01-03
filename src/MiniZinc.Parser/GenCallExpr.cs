namespace MiniZinc.Parser;

public sealed class GenCallExpr : MiniZincExpr
{
    public MiniZincExpr Expr { get; }
    public List<GenExpr> Generators { get; }
    public Token Name { get; }

    public GenCallExpr(Token name, MiniZincExpr expr, List<GenExpr> generators)
        : base(name)
    {
        Name = name;
        Expr = expr;
        Generators = generators;
    }
}
