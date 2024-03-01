namespace MiniZinc.Parser.Ast;

public sealed record GenCallExpr : IExpr
{
    public string Name { get; set; }

    public List<GeneratorExpr> From { get; set; } = new();

    public IExpr Yields { get; set; }
}
