namespace MiniZinc.Parser;

public sealed class MiniZincParseException : Exception
{
    /// <summary>
    /// A more detailed error string
    /// </summary>
    public readonly string? Trace;

    /// <summary>
    /// The token at which the error occured
    /// </summary>
    public readonly Token Location;

    public MiniZincParseException(string message, Token location, string? trace = null)
        : base(message)
    {
        Trace = trace;
        Location = location;
    }
}
