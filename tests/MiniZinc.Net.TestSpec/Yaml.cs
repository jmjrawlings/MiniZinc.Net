namespace MiniZinc.Net.Tests;

using System.Text;
using System.Text.Json.Nodes;
using YamlDotNet.Serialization;

public static class Yaml
{
    public const string TEST = "!Test";
    public const string RESULT = "!Result";
    public const string SOLUTION_SET = "!SolutionSet";
    public const string SOLUTION = "!Solution";
    public const string DURATION = "!Duration";
    public const string ERROR = "!Error";

    public static JsonNode? ParseString(string s)
    {
        var converter = new YamlConverter();
        var deserializer = new DeserializerBuilder()
            .WithTagMapping(TEST, typeof(object))
            .WithTagMapping(RESULT, typeof(object))
            .WithTagMapping(SOLUTION_SET, typeof(object))
            .WithTagMapping(SOLUTION, typeof(object))
            .WithTagMapping(DURATION, typeof(object))
            .WithTagMapping(ERROR, typeof(object))
            .WithTypeConverter(converter)
            .Build();
        var text = s.TrimEnd();
        var node = deserializer.Deserialize<JsonNode>(text);
        return node;
    }

    public static T? ParseString<T>(string s)
        where T : JsonNode
    {
        var result = ParseString(s);
        if (result is null)
            return null;

        if (result is T t)
            return t;

        throw new Exception($"Yaml string was parsed as a {result} but expected a {typeof(T)}");
    }

    public static JsonNode? ParseFile(FileInfo fi)
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString(text);
        return node;
    }

    public static T? ParseFile<T>(FileInfo fi)
        where T : JsonNode
    {
        var text = File.ReadAllText(fi.FullName, Encoding.UTF8);
        var node = ParseString<T>(text);
        return node;
    }
}
