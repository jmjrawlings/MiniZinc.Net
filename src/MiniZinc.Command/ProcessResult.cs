namespace MiniZinc.Command;

/// <summary>
/// The result of running a Process
/// </summary>
public readonly struct ProcessResult
{
    /// The command that was run
    public required string Command { get; init; }

    /// The final state
    public required ProcessStatus Status { get; init; }

    /// Any captured stdout
    public required string StdOut { get; init; }

    /// Any captured stderr
    public required string StdErr { get; init; }

    /// Time the command started running
    public required DateTimeOffset StartTime { get; init; }

    /// Time the command finished running
    public required DateTimeOffset EndTime { get; init; }

    /// Total time taken to execute
    public required TimeSpan Duration { get; init; }

    /// The exit code
    public required int ExitCode { get; init; }

    /// Did the command complete with errors?
    public bool IsError => ExitCode != 0;

    /// Did the command complete without error?
    public bool IsOk => ExitCode == 0;
}
