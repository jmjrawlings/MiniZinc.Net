namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : Expr
{
    public required Expr If { get; set; }
    public required Expr Then { get; set; }
    public List<(Expr @elseif, Expr @then)>? ElseIfs { get; set; } = new();

    public Expr? Else { get; set; }
}
