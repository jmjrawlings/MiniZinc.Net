namespace MiniZinc.Parser;

public class MiniZincParseException : Exception
{
    /// <summary>
    /// A more detailed error string
    /// </summary>
    public readonly string Trace;

    /// <summary>
    /// The token at which the error occured
    /// </summary>
    public readonly Token Token;

    public MiniZincParseException(string message, string trace, Token token)
        : base(message)
    {
        Trace = trace;
        Token = token;
    }
}
