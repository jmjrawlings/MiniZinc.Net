namespace MiniZinc.Parser.Ast;

public sealed record TupleExpr : Expr
{
    public List<Node> Fields { get; set; } = new();
}
