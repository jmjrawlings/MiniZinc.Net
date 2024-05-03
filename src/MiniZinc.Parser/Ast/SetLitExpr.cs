namespace MiniZinc.Parser.Ast;

public sealed record SetLit : Expr
{
    public List<Expr> Elements { get; set; } = new();
}
