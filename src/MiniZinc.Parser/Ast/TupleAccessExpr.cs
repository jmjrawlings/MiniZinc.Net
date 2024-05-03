namespace MiniZinc.Parser.Ast;

public sealed record TupleAccessExpr(Expr expr, int field) : Expr
{
    public Expr Expr => expr;
    public int Field => field;
}
