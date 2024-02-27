namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : Expr
{
    public List<Binding<Expr>> Fields { get; } = new();
}
