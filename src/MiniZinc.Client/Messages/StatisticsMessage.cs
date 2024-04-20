namespace MiniZinc.Client.Messages;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public sealed record StatisticsMessage : MiniZincMessage
{
    public required JsonObject Statistics { get; init; }
}
