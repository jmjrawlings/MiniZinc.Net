namespace MiniZinc.Parser.Ast;

public sealed record GeneratorExpr : Expr
{
    public List<Identifier> Names { get; set; } = new();

    public Expr From { get; set; } = Null;

    public Expr? Where { get; set; }
}
