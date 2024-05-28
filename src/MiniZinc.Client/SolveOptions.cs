using MiniZinc.Process;

namespace MiniZinc.Client;

public record SolveOptions
{
    /// <summary>
    /// Id of the desired solver (eg gecode, chuffed, highs)
    /// </summary>
    public string SolverId { get; init; } = Solver.Gecode;

    /// <summary>
    /// Extra args passed to the command line
    /// </summary>
    public Args? ExtraArgs { get; init; }

    /// <summary>
    /// If given, stops the solver after the given time
    /// </summary>
    /// <remarks>This value is passed through to minizinc via the --timeout command</remarks>
    public TimeSpan? Timeout { get; init; }
}
