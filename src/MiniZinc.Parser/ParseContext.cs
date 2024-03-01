namespace MiniZinc.Parser;

internal readonly record struct ParseContext : IDisposable, IParseContext
{
    private readonly Parser _parser;
    internal readonly Token Start;
    internal readonly AstNode Node;

    internal ParseContext(Parser parser, Token start, AstNode node)
    {
        _parser = parser;
        Start = start;
        Node = node;
    }

    public void Dispose()
    {
        _parser._context.Pop();
    }

    public long Line => Start.Line;
    public long Col => Start.Col;
    AstNode IParseContext.Node => Node;
}
