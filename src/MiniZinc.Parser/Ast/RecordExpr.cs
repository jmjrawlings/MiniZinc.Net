namespace MiniZinc.Parser.Ast;

public sealed record RecordExpr : Expr
{
    public List<(INode, INode)> Fields { get; } = new();
}
