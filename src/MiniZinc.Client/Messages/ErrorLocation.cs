namespace MiniZinc.Client.Messages;

public sealed record ErrorLocation
{
    public string Filename { get; init; }
    public int FirstLine { get; init; }
    public int FirstColumn { get; init; }
    public int LastLine { get; init; }
    public int LastColumn { get; init; }
}
