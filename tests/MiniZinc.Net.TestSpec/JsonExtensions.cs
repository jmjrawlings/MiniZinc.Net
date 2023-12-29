using System.Text.Json.Nodes;

namespace MiniZinc.Net.Tests;

public static class JsonExtensions
{
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
}
