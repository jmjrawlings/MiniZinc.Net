﻿namespace MiniZinc.Net.Process;

/// <summary>
/// The result of running a Process
/// </summary>
public readonly record struct ProcessResult
{
    /// The command that was run
    public required string Command { get; init; }

    /// The final state
    public required ProcessState State { get; init; }

    /// Time the command started running
    public required DateTime StartTime { get; init; }

    /// Time the command finished running
    public required DateTime EndTime { get; init; }

    /// Total time taken to execute
    public required TimeSpan Duration { get; init; }

    /// If captured, everything written to stdout
    public string? StdOut { get; init; }

    /// If captured, everything written to stderr
    public string? StdErr { get; init; }

    /// The exit code
    public required int ExitCode { get; init; }

    /// Did the command complete with errors?
    public bool IsError => ExitCode > 0;

    /// Did the command complete without error?
    public bool IsOk => ExitCode == 0;

    /// <summary>
    /// Throw an exception if the command was not successful
    /// </summary>
    public ProcessResult EnsureSuccess()
    {
        if (IsOk)
            return this;

        var msg = $"The command \"{Command}\" exited with code {ExitCode}:\n \"{StdErr}\"";
        throw new Exception(msg);
    }
}
