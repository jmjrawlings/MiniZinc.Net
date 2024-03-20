namespace MiniZinc.Parser.Ast;

public sealed record GeneratorExpr : IExpr
{
    public List<Identifer> Names { get; set; }

    public IExpr From { get; set; }

    public IExpr? Where { get; set; }

    public GeneratorExpr? Next { get; set; }
}
