namespace MiniZinc.Parser;

internal readonly struct ParseContext : IDisposable
{
    private Parser Parser { get; }
    public Token Start { get; }
    public string Name { get; }

    internal ParseContext(Parser parser, Token start, string name)
    {
        Parser = parser;
        Name = name;
        Start = start;
    }

    internal ParseContext(Parser parser, Token start, ScopeKind scope)
        : this(parser, start, scope.ToString()) { }

    public void Dispose()
    {
        Parser._context.Pop();
    }

    public long Line => Start.Line;
    public long Col => Start.Col;

    public override string ToString() => $"Parsing {Name} at Line {Line} Col {Col}";
}
