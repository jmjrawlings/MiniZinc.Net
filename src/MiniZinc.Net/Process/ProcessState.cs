namespace MiniZinc.Net.Process;

public enum ProcessState : byte
{
    /// Process has not been started yet
    Idle,

    /// Process is currently running
    Running,

    /// Process exited with a zero exit code
    Ok,

    /// Process exited with a non-zero exit code
    Error,

    /// Process was cancelled by the user
    Cancelled,

    /// Process has been signalled to terminate
    Signalled,
}
