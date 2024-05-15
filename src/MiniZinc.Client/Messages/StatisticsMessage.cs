namespace MiniZinc.Client.Messages;

using System.Text.Json.Nodes;

public sealed record StatisticsMessage : MiniZincMessage
{
    public required JsonObject Statistics { get; init; }
}
