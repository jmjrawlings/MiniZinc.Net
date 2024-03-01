namespace MiniZinc.Parser;

public interface IParseContext
{
    long Line { get; }
    long Col { get; }
    AstNode Node { get; }
}
