namespace LibMiniZinc.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;

public sealed record TestSpec
{
    public List<TestSuite> TestSuites { get; set; } = [];

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

    public static void ToJsonFile(TestSpec spec, FileInfo file)
    {
        var json = ToJsonString(spec);
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
