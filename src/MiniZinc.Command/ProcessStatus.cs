namespace MiniZinc.Command;

/// <summary>
/// The terminal outcome of running a process.
/// </summary>
public enum ProcessStatus : byte
{
    /// <summary>
    /// Exited with a zero exit code.
    /// </summary>
    Ok,

    /// <summary>
    /// Exited with a non-zero exit code.
    /// </summary>
    Error,

    /// <summary>
    /// Killed because cancellation was requested.
    /// </summary>
    Cancelled,
}
