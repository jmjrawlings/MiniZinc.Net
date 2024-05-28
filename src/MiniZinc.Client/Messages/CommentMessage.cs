namespace MiniZinc.Client.Messages;

using System.Text.Json.Serialization;

internal sealed record CommentMessage : MiniZincMessage
{
    [JsonPropertyName("comment")]
    public required string Comment { get; init; }
}
