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

public static class FileExtensions
{
    public static string JoinPath(this string path, string a) => Path.Combine(path, a);

    public static DirectoryInfo JoinDir(this DirectoryInfo di, string path) =>
        di.FullName.JoinPath(path).ToDirectory();
    
    public static FileInfo JoinFile(this DirectoryInfo path, string a) =>
        path.FullName.JoinPath(a).ToFile();
    
    public static FileInfo ToFile(this string path) =>
        new (path);

    public static DirectoryInfo ToDirectory(this string path) => new(path);
}
