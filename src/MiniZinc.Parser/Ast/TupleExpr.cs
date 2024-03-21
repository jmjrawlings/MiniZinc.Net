namespace MiniZinc.Parser.Ast;

public sealed record TupleExpr : Expr
{
    public List<INode> Exprs { get; set; } = new();
}
