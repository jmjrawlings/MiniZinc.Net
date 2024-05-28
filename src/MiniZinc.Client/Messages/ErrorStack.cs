namespace MiniZinc.Client.Messages;

internal sealed record ErrorStack
{
    public required ErrorLocation Location { get; init; }

    public bool IsCompIter { get; init; }

    public required string Description { get; init; }
}
