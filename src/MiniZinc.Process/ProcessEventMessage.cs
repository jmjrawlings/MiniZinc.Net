namespace MiniZinc.Process;

/// <summary>
/// Message streamed
/// </summary>
public readonly record struct ProcessEventMessage
{
    /// Time this message was created
    public required DateTime TimeStamp { get; init; }

    /// The type of event
    public required ProcessEventType EventType { get; init; }

    /// The string content of the message
    public string? Content { get; init; }
}
