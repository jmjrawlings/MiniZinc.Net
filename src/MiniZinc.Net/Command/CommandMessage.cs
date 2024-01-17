namespace MiniZinc.Net;

/// <summary>
/// The type of CommandOutput message
/// </summary>
public enum CommandOutputType
{
    /// The command has started running
    Started = 0,

    /// The message originates from stdout
    StdOut = 1,

    /// A message originates from stderr
    StdErr = 2,

    /// The command complete successfully
    Success = 3,

    ///  The command returned a non-zero exit code
    Failure = 4,
}

/// <summary>
/// Message streamed
/// </summary>
public readonly record struct CommandMessage
{
    /// Time this message was created
    public required DateTimeOffset TimeStamp { get; init; }

    /// The string content of the message
    public required string Content { get; init; }

    /// The type of message
    public required CommandOutputType Type { get; init; }

    /// The originating command
    public required string Command { get; init; }

    /// Process Id
    public required int ProcessId { get; init; }

    /// Time the command started running
    public required DateTimeOffset StartTime { get; init; }

    /// Time elapsed since the command started running
    public required TimeSpan Elapsed { get; init; }
}
