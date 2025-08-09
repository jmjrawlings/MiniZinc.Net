namespace MiniZinc.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using Core;

public sealed record TestSpec
{
    public List<TestCase> TestCases { get; set; } = [];

    public static string ToJsonString(TestSpec spec)
    {
        var json = JsonSerializer.Serialize(spec, SerializerOptions);
        return json;
    }

    private static JsonSerializerOptions? _seraliazerOptions;
    public static JsonSerializerOptions SerializerOptions
    {
        get
        {
            if (_seraliazerOptions is not null)
                return _seraliazerOptions;

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower,
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = true
            };
            var converter = new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower);
            options.Converters.Add(converter);
            _seraliazerOptions = options;
            return options;
        }
    }

    public void ToJsonFile(string path) => ToJsonFile(path.ToFile());

    public void ToJsonFile(FileInfo file)
    {
        var json = ToJsonString(this);
        File.WriteAllText(file.FullName, json);
    }

    public static TestSpec FromJsonString(string s)
    {
        var result = JsonSerializer.Deserialize<TestSpec>(s, SerializerOptions);
        return result!;
    }

    public static TestSpec FromJsonFile(FileInfo file)
    {
        var text = file.OpenText().ReadToEnd();
        var result = FromJsonString(text);
        return result;
    }
}
