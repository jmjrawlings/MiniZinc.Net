namespace MiniZinc.Net;

/// <summary>
/// Message streamed
/// </summary>
public readonly record struct ProcessMessage
{
    /// The originating command line string
    public required string Command { get; init; }

    /// Time this message was created
    public required DateTime TimeStamp { get; init; }

    /// Process Id
    public int ProcessId { get; init; }

    /// The type of message
    public required ProcessMessageType MessageType { get; init; }

    /// The string content of the message
    public string? Content { get; init; }

    /// Time the command started running
    public required DateTime StartTime { get; init; }

    /// Time elapsed since the command started running
    public TimeSpan Elapsed { get; init; }
}
