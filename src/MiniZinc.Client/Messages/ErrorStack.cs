namespace MiniZinc.Client.Messages;

public sealed record ErrorStack
{
    public ErrorLocation Location { get; init; }

    public bool IsCompIter { get; init; }

    public string Description { get; init; }
}
