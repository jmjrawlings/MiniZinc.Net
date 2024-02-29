namespace MiniZinc.Parser;

public sealed record ParseError
{
    public string Message { get; set; }
    public long Line { get; set; }
    public long Column { get; set; }
    public long Index { get; set; }
    public string Trace { get; set; }
}
