namespace MiniZinc.Client;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

/// <summary>
/// Messages sent from MiniZinc when the --json-output flag
/// is provided.  As detailed in https://docs.minizinc.dev/en/stable/json-stream.html
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SolutionOutput), typeDiscriminator: "solution")]
[JsonDerivedType(typeof(StatisticsOutput), typeDiscriminator: "statistics")]
[JsonDerivedType(typeof(CommentOutput), typeDiscriminator: "comment")]
[JsonDerivedType(typeof(TraceOutput), typeDiscriminator: "trace")]
[JsonDerivedType(typeof(ErrorOutput), typeDiscriminator: "error")]
[JsonDerivedType(typeof(WarningOutput), typeDiscriminator: "warning")]
[JsonDerivedType(typeof(StatusOutput), typeDiscriminator: "status")]
internal class JsonOutput
{
    public static readonly JsonSerializerOptions JsonSerializerOptions;

    static JsonOutput()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public static JsonOutput Deserialize(string json)
    {
        var message = JsonSerializer.Deserialize<JsonOutput>(json, JsonSerializerOptions);
        return message!;
    }
}

internal sealed class WarningOutput : ErrorOutput { }

internal sealed class StatisticsOutput : JsonOutput
{
    public required JsonObject Statistics { get; init; }
}

internal sealed class TraceOutput : JsonOutput
{
    public string Section { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

internal sealed record MiniZincErrorLocationMessage
{
    public string Filename { get; init; } = string.Empty;
    public int FirstLine { get; init; }
    public int FirstColumn { get; init; }
    public int LastLine { get; init; }
    public int LastColumn { get; init; }
}

internal sealed class CommentOutput : JsonOutput
{
    [JsonPropertyName("comment")]
    public required string Comment { get; init; }
}

internal class ErrorOutput : JsonOutput
{
    [JsonPropertyName("what")]
    public string Kind { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public MiniZincErrorLocationMessage? Location { get; init; }

    public IEnumerable<MiniZincErrorStack>? Stack { get; init; }
}

internal sealed record MiniZincErrorStack
{
    public required MiniZincErrorLocationMessage Location { get; init; }

    public bool IsCompIter { get; init; }

    public required string Description { get; init; }
}

/// <summary>
/// The solution message provided by the
/// the solver.
/// "https://www.minizinc.org/doc-latest/en/json-stream.html"
/// </summary>
internal sealed class SolutionOutput : JsonOutput
{
    public required Dictionary<string, object> Output { get; init; }

    public int? Time { get; init; }

    public List<string>? Sections { get; init; }
}

internal sealed class StatusOutput : JsonOutput
{
    public required string Status { get; init; }

    public int? Time { get; init; }
}
