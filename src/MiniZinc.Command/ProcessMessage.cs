namespace MiniZinc.Command;

/// <summary>
/// A message emitted while a process runs. Pattern-match on the concrete type:
/// <see cref="ProcessStarted"/>, <see cref="ProcessStdOut"/>,
/// <see cref="ProcessStdErr"/>, or <see cref="ProcessExited"/>.
/// </summary>
public abstract record ProcessMessage
{
    /// <summary>
    /// The id of the process that produced this message.
    /// </summary>
    public required int ProcessId { get; init; }

    /// <summary>
    /// When the message was produced.
    /// </summary>
    public required DateTimeOffset TimeStamp { get; init; }
}

/// <summary>
/// The process has started.
/// </summary>
public sealed record ProcessStarted : ProcessMessage;

/// <summary>
/// A line written to standard output.
/// </summary>
public sealed record ProcessStdOut : ProcessMessage
{
    /// <summary>
    /// The line of text, without its trailing newline.
    /// </summary>
    public required string Text { get; init; }
}

/// <summary>
/// A line written to standard error.
/// </summary>
public sealed record ProcessStdErr : ProcessMessage
{
    /// <summary>
    /// The line of text, without its trailing newline.
    /// </summary>
    public required string Text { get; init; }
}

/// <summary>
/// The process has exited.
/// </summary>
public sealed record ProcessExited : ProcessMessage
{
    /// <summary>
    /// The process exit code.
    /// </summary>
    public required int ExitCode { get; init; }
}
