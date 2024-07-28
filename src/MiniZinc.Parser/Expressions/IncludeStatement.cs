namespace MiniZinc.Parser.Syntax;

public sealed class IncludeStatement : StatementSyntax
{
    public Token Path { get; }

    public IncludeStatement(in Token start, Token path)
        : base(start)
    {
        Path = path;
    }
}
