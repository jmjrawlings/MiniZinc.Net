namespace MiniZinc.Client;

using System.Text.Json.Nodes;
using Core;
using Parser;

/// <summary>
/// An intermediate or final result from using MiniZinc
/// to solve a Model
/// </summary>
public sealed record MiniZincMessage
{
    /// <summary>
    /// The full command passed to minizinc
    /// </summary>
    public required string Command { get; init; }

    /// <summary>
    /// The solver id used to generate this message
    /// </summary>
    public required MiniZincSolver? Solver { get; init; }

    /// <summary>
    /// Timestamp this message was produced
    /// </summary>
    public required DateTimeOffset TimeStamp { get; init; }

    /// <summary>
    /// The id of the minizinc process
    /// </summary>
    public int ProcessId { get; init; }

    /// <summary>
    /// Time period from when the process was started
    /// to this update being received
    /// </summary>
    public TimeSpan TotalTime { get; init; }

    /// <summary>
    /// Status of this solution
    /// </summary>
    public SolveStatus Status { get; init; }

    /// <summary>
    /// Time period from the last iteration to this one
    /// </summary>
    public TimeSpan IterationTime { get; init; }

    /// <summary>
    /// 1-indexed number that increments each time a new solution
    /// is provided
    /// </summary>
    public int Iteration { get; init; }

    /// <summary>
    /// The full model string passed to MiniZinc
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// The variables and their assigned values from the solution
    /// </summary>
    public MiniZincData? Data { get; init; }

    /// <summary>
    /// The raw output string
    /// </summary>
    public string? Output { get; init; }

    // /// <summary>
    // /// Items from the output section
    // /// </summary>
    // public required IReadOnlyDictionary<string, string>? Outputs { get; init; }

    /// <summary>
    /// Statistics returned by the solver if requested
    /// </summary>
    public IReadOnlyDictionary<string, JsonValue>? Statistics { get; init; }

    /// <summary>
    /// Any warnings returned by the solver
    /// </summary>
    public IReadOnlyList<string>? Warnings { get; init; }

    /// <summary>
    /// A warning if its an error status
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Text content of the message
    /// </summary>
    public MiniZincExpr? Objective { get; init; }

    /// <summary>
    /// Upper or lower bound (solver-dependent)
    /// </summary>
    public MiniZincExpr? ObjectiveBound { get; init; }

    /// <summary>
    /// Absolute Gap to optimality
    /// `abs(objective - bound)`
    /// </summary>
    public MiniZincExpr? AbsoluteGapToOptimality { get; init; }

    /// <summary>
    /// Relative Gap to optimality
    /// `abs(objective - bound) / bound`
    /// </summary>
    public double? RelativeGapToOptimality { get; init; }

    /// <summary>
    /// Absolute difference between this iteration and
    /// the previous iteration
    /// `objective[i] - objective[i-1]`
    /// </summary>
    public MiniZincExpr? AbsoluteIterationGap { get; init; }

    /// <summary>
    /// Relative difference between this iteration and
    /// the previous iteration
    /// `(objective[i] - objectivep[i-1]) / objective[i-1]`
    /// </summary>
    public double? RelativeIterationGap { get; init; }

    public bool IsSolution => !IsError;

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
