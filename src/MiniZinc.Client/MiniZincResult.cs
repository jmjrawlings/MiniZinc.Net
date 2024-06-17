namespace MiniZinc.Client;

using System.Text.Json.Nodes;
using Parser.Syntax;

/// <summary>
/// An intermediate or final result from using MiniZinc
/// to solve a Model
/// </summary>
public record MiniZincResult
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
    /// Text content of the message
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Objective value
    /// </summary>
    public object? Objective { get; init; } = null;

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
    /// The assigned values to each variable in the model
    /// </summary>
    public required IReadOnlyDictionary<string, SyntaxNode>? Data { get; init; }

    /// <summary>
    /// Items from the output section
    /// </summary>
    public IReadOnlyDictionary<string, string>? Outputs { get; init; }

    /// <summary>
    /// Statistics returned by the solver if requested
    /// </summary>
    public IReadOnlyDictionary<string, JsonValue>? Statistics { get; init; }

    /// <summary>
    /// Any warnings returned by the solver
    /// </summary>
    public required IReadOnlyList<string>? Warnings { get; init; }

    /// <summary>
    /// Get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    /// <exception cref="Exception">The variable does not exists or was not of the expected type</exception>
    public U Get<U>(string id)
        where U : SyntaxNode
    {
        if (TryGet<U>(id) is not { } value)
            throw new ArgumentException(id);

        return value;
    }

    public SyntaxNode? TryGet(string id)
    {
        if (Data is null)
            return null;

        if (Data.TryGetValue(id, out var value))
            return value;

        return null;
    }

    /// <summary>
    /// Try to get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    public U? TryGet<U>(string id)
        where U : SyntaxNode
    {
        var value = TryGet(id);
        if (value is null)
            return null;

        if (value is not U u)
            throw new Exception();

        return u;
    }

    /// Get the int solution for the given variable
    public int GetInt(string id) => Get<IntLiteralSyntax>(id).Value;

    /// Get the bool solution for the given variable
    public bool GetBool(string id) => Get<BoolLiteralSyntax>(id).Value;

    /// Get the float solution for the given variable
    public decimal GetFloat(string id) => Get<FloatLiteralSyntax>(id).Value;

    /// Get the array solution for the given variable
    public IEnumerable<U> GetArray1D<U>(string id)
    {
        var array = Get<Array1DSyntax>(id);
        foreach (var node in array.Elements)
        {
            if (node is not SyntaxNode<U> literal)
                throw new Exception();
            yield return literal.Value;
        }
    }
}

/// <summary>
/// An intermediate or final result from using MiniZinc
/// to solve a Model
/// </summary>
public abstract record MiniZincResult<T> : MiniZincResult
{
    /// <summary>
    /// Text content of the message
    /// </summary>
    public new T? Objective { get; init; } = default;

    /// <summary>
    /// Upper or lower bound (solver-dependent)
    /// </summary>
    public T? ObjectiveBound { get; init; } = default;

    /// <summary>
    /// Absolute Gap to optimality
    /// `abs(objective - bound)`
    /// </summary>
    public T? AbsoluteGapToOptimality { get; init; } = default;

    /// <summary>
    /// Relative Gap to optimality
    /// `abs(objective - bound) / bound`
    /// </summary>
    public double? RelativeGapToOptimality { get; init; } = default;

    /// <summary>
    /// Absolute difference between this iteration and
    /// the previous iteration
    /// `objective[i] - objective[i-1]`
    /// </summary>
    public T? AbsoluteIterationGap { get; init; } = default;

    /// <summary>
    /// Relative difference between this iteration and
    /// the previous iteration
    /// `(objective[i] - objectivep[i-1]) / objective[i-1]`
    /// </summary>
    public double? RelativeIterationGap { get; init; } = default;
}

public sealed record IntResult : MiniZincResult<int> { }

public sealed record FloatResult : MiniZincResult<float> { }
