namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : Expr
{
    public List<Binding<IExpr>> Fields { get; } = new();
}
