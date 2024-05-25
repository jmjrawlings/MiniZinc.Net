namespace MiniZinc.Process;

/// <summary>
/// Message streamed
/// </summary>
public readonly record struct ProcessMessage
{
    /// Time this message was created
    public required DateTimeOffset TimeStamp { get; init; }

    /// The type of event
    public required ProcessEventType EventType { get; init; }

    /// The string content of the message
    public string? Content { get; init; }
}
