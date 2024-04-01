namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveItem : Item
{
    public SolveMethod Method { get; set; }
    public IExpr Objective { get; set; }
}
