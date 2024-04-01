namespace MiniZinc.Parser.Ast;

public sealed record TupleExpr : Expr
{
    public List<INode> Fields { get; set; } = new();
}
