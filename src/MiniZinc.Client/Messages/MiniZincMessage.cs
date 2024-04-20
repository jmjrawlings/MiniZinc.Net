namespace MiniZinc.Client.Messages;

using System.Text.Json;
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
    public static readonly JsonSerializerOptions JsonSerializerOptions;

    static MiniZincMessage()
    {
        JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public static MiniZincMessage Deserialize(string json)
    {
        var message = JsonSerializer.Deserialize<MiniZincMessage>(json, JsonSerializerOptions);
        return message!;
    }
}
