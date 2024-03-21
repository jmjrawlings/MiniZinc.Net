namespace MiniZinc.Parser.Ast;

public sealed record CallExpr : Expr, INamed
{
    public string Name { get; set; }
    public List<INode>? Args { get; set; }
}
