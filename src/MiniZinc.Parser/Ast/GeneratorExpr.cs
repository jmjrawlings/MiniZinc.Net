namespace MiniZinc.Parser.Ast;

public sealed record GeneratorExpr : Expr
{
    public List<Identifier> Names { get; set; }

    public Expr From { get; set; }

    public Expr? Where { get; set; }
}
