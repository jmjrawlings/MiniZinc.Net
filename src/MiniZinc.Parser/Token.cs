namespace MiniZinc.Parser;

internal readonly struct Token
{
    public readonly TokenKind Kind;
    public readonly uint Line;
    public readonly uint Col;
    public readonly uint Start;
    public readonly uint Length;
    public readonly int Int;
    public readonly string? String;
    public readonly double Double;

    public Token(
        TokenKind kind,
        uint line,
        uint col,
        uint start,
        uint length,
        int i = default,
        double d = default,
        string? s = default
    )
    {
        Kind = kind;
        Line = line;
        Col = col;
        Start = start;
        Length = length;
        Int = i;
        String = s;
        Double = d;
    }

    public override string ToString() =>
        $"{Kind} {String} | Line {Line}, Col {Col}, Start {Start}, End {Start + Length}, Len: {Length}";
}
