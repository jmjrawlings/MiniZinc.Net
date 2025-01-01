namespace MiniZinc.Parser.Syntax;

public sealed class IncludeStatement : Statement
{
    public Token Path { get; }

    public IncludeStatement(in Token start, Token path)
        : base(start)
    {
        Path = path;
    }
}
