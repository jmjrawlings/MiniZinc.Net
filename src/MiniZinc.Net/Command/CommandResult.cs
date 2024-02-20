namespace MiniZinc.Net;

/// <summary>
/// The result of running a Command
/// </summary>
public readonly record struct CommandResult
{
    /// The command that was run
    public required string Command { get; init; }

    /// Time the command started running
    public required DateTimeOffset StartTime { get; init; }

    /// Time the command finished running
    public required DateTimeOffset EndTime { get; init; }

    /// Total time taken to execute
    public required TimeSpan Duration { get; init; }

    /// Everything written to stdout
    public required string StdOut { get; init; }

    /// Everything written to stderr
    public required string StdErr { get; init; }

    /// The exit code
    public required int ExitCode { get; init; }

    /// Did the command complete with errors?
    public bool IsError => ExitCode > 0;

    /// Did the command complete without error?
    public bool IsOk => ExitCode == 0;

    /// <summary>
    /// Throw an exception if the command was not successful
    /// </summary>
    public CommandResult EnsureSuccess()
    {
        if (IsOk)
            return this;

        var msg = $"The command \"{Command}\" exited with code {ExitCode}:\n \"{StdErr}\"";
        throw new Exception(msg);
    }
}
