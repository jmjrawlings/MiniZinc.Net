namespace MiniZinc.Parser.Ast;

/// <summary>
/// solve maximize a;
/// </summary>
public record SolveStatement : Node
{
    public SolveMethod Method { get; set; }

    public Expr Objective { get; set; } = Expr.Null;
}
