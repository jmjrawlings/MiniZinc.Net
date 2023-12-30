using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;

namespace MiniZinc.Net.Tests;

public static class Json
{
    public static string SerializeToString(object obj)
    {
        var json = JsonSerializer.Serialize(obj, Options);
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
        var result = JsonSerializer.Deserialize<T>(s, Options);
        Guard.IsNotNull(result);
        return result;
    }

    public static T DeserializeFromFile<T>(FileInfo file)
    {
        var text = file.OpenText().ReadToEnd();
        var result = DeserializeFromString<T>(text);
        return result;
    }

    /// <summary>
    /// Extract the values of the given json array
    /// </summary>
    public static List<T> ToList<T>(this JsonNode node, Func<JsonNode, T> f)
    {
        if (node is JsonArray arr)
        {
            List<T> list = new();
            foreach (var item in arr)
            {
                if (item is null)
                    continue;

                var val = f(item);
                list.Add(val);
            }
            return list;
        }
        throw new Exception("Node was not an array");
    }

    /// <summary>
    /// Extract the values of the given json array
    /// </summary>
    public static List<T> ToList<T>(this JsonNode node)
    {
        if (node is JsonArray arr)
        {
            List<T> list = new();
            foreach (var item in arr)
            {
                var val = item!.GetValue<T>();
                list.Add(val);
            }

            return list;
        }
        throw new Exception("Node was not an array");
    }

    /// <summary>
    /// Extract the values of the given json array, returning
    /// null if the key node is null or the empty list
    /// </summary>
    public static List<T>? ToNonEmptyList<T>(this JsonNode node)
    {
        List<T>? ret = null;

        if (node is JsonArray arr)
        {
            foreach (var item in arr)
            {
                ret ??= new List<T>();
                var val = item!.GetValue<T>();
                ret.Add(val);
            }

            return ret;
        }
        throw new Exception("Node was not an array");
    }

    public static T? GetValue<T>(this JsonNode node, string key)
        where T : notnull
    {
        var item = node[key];
        if (item is null)
            return default;

        var value = item.GetValue<T>();
        return value;
    }

    public static string? GetString(this JsonNode node, string key) => node.GetValue<string>(key);

    public static string GetStringExn(this JsonNode node, string key) =>
        node.GetValue<string>(key) ?? throw new Exception();

    private static JsonSerializerOptions? _options;
    public static JsonSerializerOptions Options
    {
        get
        {
            if (_options is not null)
                return _options;

            var options = new JsonSerializerOptions();
            // options.WriteIndented = true;
            options.WriteIndented = false;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            var converter = new JsonStringEnumConverter();
            options.Converters.Add(converter);
            _options = options;
            return options;
        }
    }
}
