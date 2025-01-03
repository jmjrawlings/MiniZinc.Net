namespace MiniZinc.Parser;

public sealed class IncludeItem : MiniZincItem
{
    public Token Path { get; }

    public IncludeItem(in Token start, Token path)
        : base(start)
    {
        Path = path;
    }
}
