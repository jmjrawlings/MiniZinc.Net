namespace MiniZinc.Parser.Syntax;

public sealed class GenCallExpr : Expr
{
    public Expr Expr { get; }
    public List<GenExpr> Generators { get; }
    public Token Name { get; }

    public GenCallExpr(Token name, Expr expr, List<GenExpr> generators)
        : base(name)
    {
        Name = name;
        Expr = expr;
        Generators = generators;
    }
}
