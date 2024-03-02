namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveItem : IStatement, IAnnotations
{
    public SolveMethod Method { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public IExpr? Objective { get; set; }
}
