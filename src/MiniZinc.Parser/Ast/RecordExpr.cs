namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : Expr
{
    public List<Binding<INode>> Fields { get; } = new();
}
