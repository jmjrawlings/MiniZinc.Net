namespace MiniZinc.Parser.Ast;

public abstract record Expr : Node
{
    public static Expr Wildcard => new WildCardExpr();

    public static Expr Empty => new EmptyExpr();

    public static readonly Expr Null = new NullExpr();
}
