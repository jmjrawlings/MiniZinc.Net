namespace MiniZinc.Client.Messages;

using System.Text.Json.Nodes;

internal sealed record StatisticsMessage : MiniZincMessage
{
    public required JsonObject Statistics { get; init; }
}
