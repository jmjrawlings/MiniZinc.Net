using System.Text.Json.Serialization;

namespace MiniZinc.Client.Messages;

/// <summary>
/// The solution message provided by the
/// the solver. See <see cref="https://www.minizinc.org/doc-latest/en/json-stream.html"/>
/// </summary>
public sealed record SolutionMessage : MiniZincMessage
{
    public int? Time { get; init; }

    public List<string>? Sections { get; init; }

    public Dictionary<string, object>? Output { get; init; }
}
