namespace MiniZinc.Parser;

public sealed class ParseException(Token Token, string Message) : Exception
{
}
