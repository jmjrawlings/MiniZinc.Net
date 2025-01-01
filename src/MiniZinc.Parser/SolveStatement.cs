namespace MiniZinc.Parser.Syntax;

/// <summary>
/// solve maximize a;
/// </summary>
public sealed class SolveStatement : Statement
{
    public SolveMethod Method { get; }
    public Expr? Objective { get; }

    /// <summary>
    /// solve maximize a;
    /// </summary>
    public SolveStatement(in Token start, SolveMethod method, Expr? objective = null)
        : base(start)
    {
        Method = method;
        Objective = objective;
    }
}
