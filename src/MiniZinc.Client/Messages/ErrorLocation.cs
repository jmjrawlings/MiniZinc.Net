namespace MiniZinc.Client.Messages;

internal sealed record ErrorLocation
{
    public string Filename { get; init; } = string.Empty;
    public int FirstLine { get; init; }
    public int FirstColumn { get; init; }
    public int LastLine { get; init; }
    public int LastColumn { get; init; }
}
