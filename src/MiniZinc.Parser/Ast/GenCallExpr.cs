namespace MiniZinc.Parser.Ast;

public sealed record GenCallExpr : IExpr
{
    public string Name { get; set; }

    public List<GeneratorExpr> Generators { get; set; }

    public IExpr Expr { get; set; }
}
