namespace MiniZinc.Client.Messages;

using System.Text.Json.Serialization;

public record ErrorMessage : MiniZincMessage
{
    [JsonPropertyName("what")]
    public string Kind { get; init; }

    public string Message { get; init; }

    public ErrorLocation? Location { get; init; }

    public IEnumerable<ErrorStack> Stack { get; init; }
}
