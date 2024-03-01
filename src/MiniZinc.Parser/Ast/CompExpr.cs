namespace MiniZinc.Parser.Ast;

public record CompExpr : IExpr
{
    public IExpr Yields { get; set; }
    public bool IsSet { get; set; }
    public List<GeneratorExpr> From { get; set; }
}