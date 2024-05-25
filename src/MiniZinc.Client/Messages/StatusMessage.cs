namespace MiniZinc.Client.Messages;

public sealed record StatusMessage : MiniZincMessage
{
    public required string Status { get; init; }

    public int? Time { get; init; }
}
