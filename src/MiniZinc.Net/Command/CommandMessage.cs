namespace MiniZinc.Net;

/// <summary>
/// Message streamed
/// </summary>
public readonly record struct CommandMessage
{
    /// Process Id
    public required int ProcessId { get; init; }

    /// The originating command string
    public required string Command { get; init; }

    /// Time this message was created
    public required DateTimeOffset TimeStamp { get; init; }

    /// The type of message
    public required CommandMessageType MessageType { get; init; }

    /// The string content of the message
    public required string? Content { get; init; }

    /// Time the command started running
    public required DateTimeOffset StartTime { get; init; }

    /// Time elapsed since the command started running
    public required TimeSpan Elapsed { get; init; }
}
