namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveItem : IStatement, IAnnotations
{
    public SolveMethod SolveMethod { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public IExpr? Expr { get; set; }
}