namespace MiniZinc.Client.Messages;

public sealed record StatusMessage : MiniZincMessage
{
    public SolverStatus Status { get; init; }

    public int? Time { get; init; }
}
