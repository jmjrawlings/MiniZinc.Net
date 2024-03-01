namespace MiniZinc.Parser;

public sealed class ParseException : Exception
{
    public IEnumerable<IParseContext> Context;

    internal ParseException(IEnumerable<IParseContext> context)
    {
        Context = context;
    }
}
