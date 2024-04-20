namespace MiniZinc.Parser.Ast;

public sealed record GenCallExpr : Expr
{
    public required string Name { get; set; }

    public required List<GeneratorExpr> Generators { get; set; }

    public Expr Expr { get; set; } = Null;
}
