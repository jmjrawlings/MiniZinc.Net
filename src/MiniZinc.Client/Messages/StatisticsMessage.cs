namespace MiniZinc.Client.Messages;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public sealed record StatisticsMessage : MiniZincMessage
{
    public JsonObject Statistics { get; init; }
}
