namespace MiniZinc.Parser.Ast;

/// <summary>
/// An array or set comprehension
/// </summary>
public record CompExpr : IExpr
{
    public IExpr Yields { get; set; }
    public bool IsSet { get; set; }
    public List<GeneratorExpr> From { get; set; }
}
