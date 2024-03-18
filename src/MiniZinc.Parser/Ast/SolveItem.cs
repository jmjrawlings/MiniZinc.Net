namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveItem : IAnnotations
{
    public SolveMethod Method { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public IExpr Objective { get; set; }
}
