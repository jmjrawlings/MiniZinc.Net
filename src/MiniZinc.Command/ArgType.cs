namespace MiniZinc.Command;

/// <summary>
/// Type of argument
/// </summary>
internal enum ArgType : byte
{
    /// <summary>
    /// --flag
    /// </summary>
    FlagOnly,

    /// <summary>
    /// value
    /// </summary>
    ValueOnly,

    /// <summary>
    /// --flag value
    /// </summary>
    FlagOptionSpace,

    /// <summary>
    /// --flag=value
    /// </summary>
    FlagOptionEqual
}
