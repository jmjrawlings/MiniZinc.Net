namespace MiniZinc.Net;

public static class Prelude
{
    public static Dictionary<K, R> Map<K, V, R>(this Dictionary<K, V> dict, Func<V, R> f)
        where K : notnull
    {
        Dictionary<K, R> result = new();
        foreach (var kv in dict)
        {
            result[kv.Key] = f(kv.Value);
        }

        return result;
    }
}
