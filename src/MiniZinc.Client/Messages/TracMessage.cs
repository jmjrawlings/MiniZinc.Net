namespace MiniZinc.Client.Messages;

public sealed record TraceMessage : MiniZincMessage
{
    public string Section { get; init; }

    public string Message { get; init; }
}
