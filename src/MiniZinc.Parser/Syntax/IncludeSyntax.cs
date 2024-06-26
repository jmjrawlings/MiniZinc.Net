namespace MiniZinc.Parser.Syntax;

public sealed class IncludeSyntax : StatementSyntax
{
    public Token Path { get; }

    public IncludeSyntax(in Token start, Token path)
        : base(start)
    {
        Path = path;
    }
}
