namespace MiniZinc.Client;

using Core;
using Parser;

/// <summary>
/// An intermediate or final result from using MiniZinc
/// to solve a Model
/// </summary>
public sealed record MiniZincResult
{
    /// <summary>
    /// The full command passed to minizinc
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// The id of the minizinc process
    /// </summary>
    public required int ProcessId { get; init; }

    /// <summary>
    /// The solver id used to generate this message
    /// </summary>
    public required string SolverId { get; init; }

    /// <summary>
    /// Time period from when the process was started
    /// to this update being received
    /// </summary>
    public required TimePeriod TotalTime { get; init; }

    /// <summary>
    /// Status of this solution
    /// </summary>
    public required SolveStatus Status { get; init; }

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
    /// The variables and their assigned values from the solution
    /// </summary>
    public required MiniZincData Data { get; init; }

    // /// <summary>
    // /// Items from the output section
    // /// </summary>
    // public required IReadOnlyDictionary<string, string>? Outputs { get; init; }

    /// <summary>
    /// Statistics returned by the solver if requested
    /// </summary>
    public required IReadOnlyDictionary<string, object>? Statistics { get; init; }

    /// <summary>
    /// Any warnings returned by the solver
    /// </summary>
    public required IReadOnlyList<string>? Warnings { get; init; }

    /// <summary>
    /// A warning if its an error status
    /// </summary>
    public required string? Error { get; init; }

    /// <summary>
    /// Text content of the message
    /// </summary>
    public required DataSyntax? Objective { get; init; }

    /// <summary>
    /// Upper or lower bound (solver-dependent)
    /// </summary>
    public required DataSyntax? ObjectiveBound { get; init; }

    /// <summary>
    /// Absolute Gap to optimality
    /// `abs(objective - bound)`
    /// </summary>
    public required DataSyntax? AbsoluteGapToOptimality { get; init; }

    /// <summary>
    /// Relative Gap to optimality
    /// `abs(objective - bound) / bound`
    /// </summary>
    public required double? RelativeGapToOptimality { get; init; }

    /// <summary>
    /// Absolute difference between this iteration and
    /// the previous iteration
    /// `objective[i] - objective[i-1]`
    /// </summary>
    public required DataSyntax? AbsoluteIterationGap { get; init; }

    /// <summary>
    /// Relative difference between this iteration and
    /// the previous iteration
    /// `(objective[i] - objectivep[i-1]) / objective[i-1]`
    /// </summary>
    public required double? RelativeIterationGap { get; init; }

    public bool IsSuccess => !IsError;

    public bool IsError =>
        Status switch
        {
            SolveStatus.Error => true,
            SolveStatus.AssertionError => true,
            SolveStatus.EvaluationError => true,
            SolveStatus.SyntaxError => true,
            SolveStatus.TypeError => true,
            SolveStatus.Timeout => true,
            SolveStatus.Unsatisfiable => true,
            _ => false
        };
}
