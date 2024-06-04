namespace MiniZinc.Client;

using System.Text.Json.Nodes;
using Command;
using Parser.Syntax;

public record Solution
{
    /// <summary>
    /// The command line string used to start the solver
    /// </summary>
    public required Command Command { get; init; }

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
    /// Raw minizinc text 
    /// </summary>
    public required string Text { get; init; }
    
    /// <summary>
    /// The assigned values to each variable in the model
    /// </summary>
    public required IReadOnlyDictionary<string, SyntaxNode>? Data { get; init; }

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

    /// <summary>
    /// Get the solution assigned to the given variable
    /// </summary>
    /// <param name="id">Name of the model variable</param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="Exception">The variable does not exists or was not of the expected type</exception>
    public T Get<T>(string id)
        where T : SyntaxNode
    {
        if (Data is not null)
            if (Data.TryGetValue(id, out var var))
                if (var is T t)
                    return t;

        throw new Exception();
    }

    /// Get the int solution for the given variable
    public int GetInt(string id) => Get<IntLiteralSyntax>(id).Value;

    /// Get the bool solution for the given variable
    public bool GetBool(string id) => Get<BoolLiteralSyntax>(id).Value;

    /// Get the float solution for the given variable
    public decimal GetFloat(string id) => Get<FloatLiteralSyntax>(id).Value;

    /// Get the array solution for the given variable
    public IEnumerable<T> GetArray1D<T>(string id)
    {
        var array = Get<Array1DSyntax>(id);
        foreach (var node in array.Elements)
        {
            if (node is not SyntaxNode<T> literal)
                throw new Exception();
            yield return literal.Value;
        }
    }
}
