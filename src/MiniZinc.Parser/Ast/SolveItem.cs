namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveItem : Expr
{
    public SolveMethod Method { get; set; }
    public INode Objective { get; set; }
}
