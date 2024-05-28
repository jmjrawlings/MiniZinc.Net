namespace MiniZinc.Client.Messages;

internal sealed record StatusMessage : MiniZincMessage
{
    public required string Status { get; init; }

    public int? Time { get; init; }
}
