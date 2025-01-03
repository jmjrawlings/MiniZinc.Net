namespace MiniZinc.Parser;

/// <summary>
/// solve maximize a;
/// </summary>
public sealed class SolveItem : MiniZincItem
{
    public SolveMethod Method { get; }
    public MiniZincExpr? Objective { get; }

    /// <summary>
    /// solve maximize a;
    /// </summary>
    public SolveItem(in Token start, SolveMethod method, MiniZincExpr? objective = null)
        : base(start)
    {
        Method = method;
        Objective = objective;
    }
}
