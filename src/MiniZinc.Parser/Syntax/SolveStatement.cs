namespace MiniZinc.Parser.Syntax;

/// <summary>
/// solve maximize a;
/// </summary>
public sealed class SolveStatement : StatementSyntax
{
    public SolveMethod Method { get; }
    public ExpressionSyntax? Objective { get; }

    /// <summary>
    /// solve maximize a;
    /// </summary>
    public SolveStatement(in Token start, SolveMethod method, ExpressionSyntax? objective = null)
        : base(start)
    {
        Method = method;
        Objective = objective;
    }
}
