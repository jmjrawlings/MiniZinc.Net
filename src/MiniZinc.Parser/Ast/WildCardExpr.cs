namespace MiniZinc.Parser.Ast;

public sealed record WildCardExpr : Expr
{
    public override string ToString() => "_";
}
