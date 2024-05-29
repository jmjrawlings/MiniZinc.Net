namespace MiniZinc.Client;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

/// <summary>
/// MiniZinc JSON output
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(MiniZincSolutionMessage), typeDiscriminator: "solution")]
[JsonDerivedType(typeof(MiniZincStatMessage), typeDiscriminator: "statistics")]
[JsonDerivedType(typeof(MiniZincCommentMessage), typeDiscriminator: "comment")]
[JsonDerivedType(typeof(MiniZincTraceMessage), typeDiscriminator: "trace")]
[JsonDerivedType(typeof(MiniZincErrorMessage), typeDiscriminator: "error")]
[JsonDerivedType(typeof(MiniZincWarningMessage), typeDiscriminator: "warning")]
[JsonDerivedType(typeof(MiniZincStatusMessage), typeDiscriminator: "status")]
internal record MiniZincJsonMessage
{
    public static readonly JsonSerializerOptions JsonSerializerOptions;

    static MiniZincJsonMessage()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public static MiniZincJsonMessage Deserialize(string json)
    {
        var message = JsonSerializer.Deserialize<MiniZincJsonMessage>(json, JsonSerializerOptions);
        return message!;
    }
}

sealed record MiniZincWarningMessage : MiniZincErrorMessage { }

sealed record MiniZincStatMessage : MiniZincJsonMessage
{
    public required JsonObject Statistics { get; init; }
}

sealed record MiniZincTraceMessage : MiniZincJsonMessage
{
    public string Section { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

sealed record MiniZincErrorLocationMessage
{
    public string Filename { get; init; } = string.Empty;
    public int FirstLine { get; init; }
    public int FirstColumn { get; init; }
    public int LastLine { get; init; }
    public int LastColumn { get; init; }
}

sealed record MiniZincCommentMessage : MiniZincJsonMessage
{
    [JsonPropertyName("comment")]
    public required string Comment { get; init; }
}

record MiniZincErrorMessage : MiniZincJsonMessage
{
    [JsonPropertyName("what")]
    public string Kind { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;

    public MiniZincErrorLocationMessage? Location { get; init; }

    public IEnumerable<MiniZincErrorStack>? Stack { get; init; }
}

sealed record MiniZincErrorStack
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
sealed record MiniZincSolutionMessage : MiniZincJsonMessage
{
    public int? Time { get; set; }

    public List<string>? Sections { get; set; }

    public Dictionary<string, object>? Output { get; set; }
}

sealed record MiniZincStatusMessage : MiniZincJsonMessage
{
    public required string Status { get; init; }

    public int? Time { get; init; }
}
