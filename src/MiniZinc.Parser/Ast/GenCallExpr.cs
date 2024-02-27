namespace MiniZinc.Parser.Ast;

public sealed record GenCallExpr : Expr
{
    public string Name { get; set; }

    public List<GeneratorExpr> Generators { get; set; }

    public Expr Expr { get; set; }
}
