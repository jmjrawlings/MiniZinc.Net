namespace MiniZinc.Client.Messages;

public sealed record TraceMessage : MiniZincMessage
{
    public string Section { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
