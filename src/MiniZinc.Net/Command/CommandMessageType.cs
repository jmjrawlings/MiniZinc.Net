namespace MiniZinc.Net;

/// <summary>
/// The type of CommandOutput message
/// </summary>
public enum CommandMessageType : byte
{
    /// The command has started running
    Started,

    /// The message originates from stdout
    StdOut,

    /// A message originates from stderr
    StdErr,

    /// The command complete successfully
    Success,

    ///  The command returned a non-zero exit code
    Failure,
}
