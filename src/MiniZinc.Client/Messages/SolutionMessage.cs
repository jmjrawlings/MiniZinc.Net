namespace MiniZinc.Client.Messages;

/// <summary>
/// The solution message provided by the
/// the solver. 
/// "https://www.minizinc.org/doc-latest/en/json-stream.html"
/// </summary>
public sealed record SolutionMessage : MiniZincMessage
{
    public int? Time { get; set; }

    public List<string>? Sections { get; set; }

    public Dictionary<string, object>? Output { get; set; }
}
