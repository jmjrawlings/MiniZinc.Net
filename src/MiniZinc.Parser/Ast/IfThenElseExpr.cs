namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : Expr
{
    public Expr If { get; set; }
    public Expr Then { get; set; }
    public List<(Expr @elseif, Expr @then)>? ElseIfs { get; set; } = new();

    public Expr? Else { get; set; }
}
