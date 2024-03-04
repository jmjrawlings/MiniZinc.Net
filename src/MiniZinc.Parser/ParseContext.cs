namespace MiniZinc.Parser;

internal readonly record struct ParseContext : IDisposable, IParseContext
{
    private readonly Parser _parser;
    internal readonly Token Start;
    internal readonly ScopeKind Scope;

    internal ParseContext(Parser parser, Token start, ScopeKind scope)
    {
        _parser = parser;
        Start = start;
        Scope = scope;
    }

    public void Dispose()
    {
        _parser._context.Pop();
    }

    public long Line => Start.Line;
    public long Col => Start.Col;
    ScopeKind IParseContext.Scope => Scope;

    public override string ToString() => $"Parsing {Scope} at Line {Line} Col {Col}";
}
