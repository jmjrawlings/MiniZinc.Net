namespace MiniZinc.Client.Messages;

using System.Text.Json.Serialization;

internal record ErrorMessage : MiniZincMessage
{
    [JsonPropertyName("what")]
    public string Kind { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public ErrorLocation? Location { get; init; }

    public IEnumerable<ErrorStack>? Stack { get; init; }
}
