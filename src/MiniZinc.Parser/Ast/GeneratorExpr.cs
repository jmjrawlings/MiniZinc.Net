namespace MiniZinc.Parser.Ast;

public sealed record GeneratorExpr : IExpr
{
    public List<string?> Names { get; set; }

    public IExpr From { get; set; }

    public IExpr? Where { get; set; }
}
