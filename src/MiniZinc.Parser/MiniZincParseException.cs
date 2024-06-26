namespace MiniZinc.Parser;

public class MiniZincParseException : Exception
{
    /// <summary>
    /// A more detailed error string
    /// </summary>
    public readonly string? Trace;

    /// <summary>
    /// The token at which the error occured
    /// </summary>
    public readonly Token Token;

    public MiniZincParseException(string message, Token token, string? trace = null)
        : base(message)
    {
        Trace = trace;
        Token = token;
    }
}
