namespace MiniZinc.Client;

using System.Text.Json.Nodes;

public record Solution
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
    /// Time period from when the solver was started
    /// to when this solution was received
    /// </summary>
    public required TimePeriod TotalTime { get; init; }

    /// <summary>
    /// Time period from the last iteration to this one
    /// </summary>
    public required TimePeriod IterationTime { get; init; }

    /// <summary>
    /// 1-indexed number that increments each time a new solution
    /// is provided
    /// </summary>
    public required int Iteration { get; init; }

    /// <summary>
    /// Status of this solution
    /// </summary>
    public required SolveStatus Status { get; init; }

    /// <summary>
    /// Objective value
    /// </summary>
    public required int? Objective { get; init; }

    /// <summary>
    /// Upper or lower bound (solver-dependent)
    /// </summary>
    public required int? Bound { get; init; }

    /// <summary>
    /// Gap to optimality
    /// </summary>
    public required int? AbsoluteGap { get; init; }

    /// <summary>
    /// Gap to optimality as a percent
    /// </summary>
    public required float? RelativeGap { get; init; }

    /// <summary>
    /// Difference in the objective between this solution
    /// and the previous one
    /// </summary>
    public required int? AbsoluteDelta { get; init; }

    /// <summary>
    /// Difference in the relative gaps between this
    /// solution and the previous one
    /// </summary>
    public required float? RelativeDelta { get; init; }

    /// <summary>
    /// Variables and their solutions
    /// </summary>
    public required IReadOnlyDictionary<string, object>? Variables { get; init; }

    /// <summary>
    /// Items from the output section
    /// </summary>
    public required IReadOnlyDictionary<string, string>? Outputs { get; init; }

    /// <summary>
    /// Statistics returned by the solver if requested
    /// </summary>
    public required IReadOnlyDictionary<string, JsonValue>? Statistics { get; init; }

    /// <summary>
    /// Any warnings returned by the solver
    /// </summary>
    public required JsonObject? Warnings { get; init; }
}
