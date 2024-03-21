namespace MiniZinc.Parser.Ast;

public sealed record GenCallExpr : INode
{
    public string Name { get; set; }

    public List<GeneratorExpr> Generators { get; set; }

    public INode Expr { get; set; }
}
