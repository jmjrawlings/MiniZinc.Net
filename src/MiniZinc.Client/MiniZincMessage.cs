namespace MiniZinc.Client;

/// <summary>
/// A message returned by the <see cref="MiniZincClient"/>
/// </summary>
public record MiniZincMessage
{
    /// <summary>
    /// The command line string used to start the solver
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// The associated process id from the solver
    /// </summary>
    public required int ProcessId { get; init; }

    /// <summary>
    /// The solver id used to generate this message
    /// </summary>
    public required string SolverId { get; init; }

    /// <summary>
    /// Time period from when the command was run to
    /// when this message was received
    /// </summary>
    public required TimePeriod TotalTime { get; init; }

    /// <summary>
    /// Status of this solution
    /// </summary>
    public required SolveStatus Status { get; init; }
}
