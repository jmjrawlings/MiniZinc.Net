namespace MiniZinc.Parser.Ast;

public record ArrayExpr : Expr
{
    public List<Expr> Elements { get; set; } = new();
    public int N => Elements.Count;
}