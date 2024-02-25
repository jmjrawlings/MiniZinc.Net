using System.Text.Json;

namespace MiniZinc.Client.Messages;

using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(SolutionMessage), typeDiscriminator: "solution")]
[JsonDerivedType(typeof(StatisticsMessage), typeDiscriminator: "statistics")]
[JsonDerivedType(typeof(CommentMessage), typeDiscriminator: "comment")]
[JsonDerivedType(typeof(TraceMessage), typeDiscriminator: "trace")]
[JsonDerivedType(typeof(ErrorMessage), typeDiscriminator: "error")]
[JsonDerivedType(typeof(WarningMessage), typeDiscriminator: "warning")]
[JsonDerivedType(typeof(StatusMessage), typeDiscriminator: "status")]
public record MiniZincMessage
{
    private static JsonSerializerOptions? _jsonSerializerOptions;

    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            if (_jsonSerializerOptions is not null)
                return _jsonSerializerOptions;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            return _jsonSerializerOptions;
        }
    }

    public static MiniZincMessage Deserialize(string json)
    {
        var message = JsonSerializer.Deserialize<MiniZincMessage>(json, _jsonSerializerOptions);
        return message;
    }
}
