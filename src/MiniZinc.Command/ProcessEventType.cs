namespace MiniZinc.Command;

/// <summary>
/// The type of CommandOutput message
/// </summary>
public enum ProcessEventType : byte
{
    /// The command has started running
    Started,

    /// The message originates from stdout
    StdOut,

    /// A message originates from stderr
    StdErr,

    /// The command exited
    Exited,
}
