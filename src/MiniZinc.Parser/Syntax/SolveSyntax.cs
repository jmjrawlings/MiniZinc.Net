namespace MiniZinc.Parser.Syntax;

/// <summary>
/// solve maximize a;
/// </summary>
public sealed class SolveSyntax : StatementSyntax
{
    public SolveMethod Method { get; }
    public SyntaxNode? Objective { get; }

    /// <summary>
    /// solve maximize a;
    /// </summary>
    public SolveSyntax(in Token start, SolveMethod method, ExpressionSyntax? objective = null)
        : base(start)
    {
        Method = method;
        Objective = objective;
    }
}
