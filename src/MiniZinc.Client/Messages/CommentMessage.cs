namespace MiniZinc.Client.Messages;

using System.Text.Json.Serialization;

public sealed record CommentMessage : MiniZincMessage
{
    [JsonPropertyName("comment")]
    public required string Comment { get; init; }
}
