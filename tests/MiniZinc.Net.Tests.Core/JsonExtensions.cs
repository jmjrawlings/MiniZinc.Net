namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

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
    public static List<T> AsListOf<T>(this JsonNode? node)
    {
        var lst = new List<T>();
        if (node is JsonArray arr)
        {
            foreach (var item in arr)
                if (item is T t)
                    lst.Add(t);
        }
        else if (node is T t)
            lst.Add(t);

        return lst;
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

    public static T? TryGetValue<T>(this JsonNode? node)
        where T : notnull
    {
        if (node is null)
            return default;

        var value = node.GetValue<T>();
        return value;
    }

    public static T? TryGetValue<T>(this JsonNode? node, string key)
        where T : notnull
    {
        var item = node?[key];
        if (item is null)
            return default;

        var value = item.GetValue<T>();
        return value;
    }

    public static T GetValue<T>(this JsonNode? node, string key)
        where T : notnull => node.TryGetValue<T>(key) ?? throw new Exception();

    /// <summary>
    /// Pop the given key from a node
    /// </summary>
    public static JsonNode? Pop(this JsonNode? node, string key)
    {
        if (node is JsonObject obj)
        {
            if (obj.ContainsKey(key))
            {
                var item = obj[key];
                obj.Remove(key);
                return item;
            }
        }

        return null;
    }

    public static T? Match<T>(
        this JsonNode? node,
        Func<JsonObject, T>? obj = null,
        Func<JsonArray, T>? arr = null,
        Func<JsonValue, T>? val = null
    )
    {
        T? result = node switch
        {
            JsonArray x when arr is not null => arr(x),
            JsonObject x when obj is not null => obj(x),
            JsonValue x when val is not null => val(x),
            _ => default
        };

        return result;
    }

    public static void Walk(
        this JsonNode? node,
        Action<JsonObject>? ifObj = null,
        Action<JsonArray>? ifArr = null,
        Action<JsonValue>? ifVal = null
    )
    {
        switch (node)
        {
            case null:
                break;
            case JsonArray arr:
                ifArr?.Invoke(arr);
                foreach (var item in arr)
                {
                    Walk(item, ifObj, ifArr, ifVal);
                }
                break;
            case JsonObject obj:
                ifObj?.Invoke(obj);
                foreach (var kv in obj)
                {
                    Walk(kv.Value, ifObj, ifArr, ifVal);
                }

                break;
            case JsonValue val:
                ifVal?.Invoke(val);
                break;
        }
    }
}
