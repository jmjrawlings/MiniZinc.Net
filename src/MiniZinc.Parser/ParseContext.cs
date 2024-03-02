namespace MiniZinc.Parser;

internal readonly record struct ParseContext : IDisposable, IParseContext
{
    private readonly Parser _parser;
    internal readonly Token Start;
    internal readonly NodeKind Node;

    internal ParseContext(Parser parser, Token start, NodeKind node)
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
    NodeKind IParseContext.Node => Node;

    public override string ToString() => $"Parsing {Node} at Line {Line} Col {Col}";
}
