namespace MiniZinc.Parser.Ast;

public sealed record RecordAccess(Expr expr, string field) : Expr
{
    public Expr Expr => expr;
    public string Field => field;
}
