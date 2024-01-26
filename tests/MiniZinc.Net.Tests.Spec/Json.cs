namespace MiniZinc.Net.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public static class Json
{
    public static string SerializeToString(object obj)
    {
        var json = JsonSerializer.Serialize(obj, SerializerOptions);
        return json;
    }

    public static FileInfo SerializeToFile(object obj, FileInfo file)
    {
        var json = SerializeToString(obj);
        File.WriteAllText(file.FullName, json);
        return file;
    }

    public static T DeserializeFromString<T>(string s)
    {
        var result = JsonSerializer.Deserialize<T>(s, SerializerOptions);
        return result;
    }

    public static T DeserializeFromFile<T>(FileInfo file)
    {
        var text = file.OpenText().ReadToEnd();
        var result = DeserializeFromString<T>(text);
        return result;
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
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            var converter = new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower);
            options.Converters.Add(converter);
            _seraliazerOptions = options;
            return options;
        }
    }
}
