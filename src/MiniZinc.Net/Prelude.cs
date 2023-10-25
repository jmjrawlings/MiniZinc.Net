namespace MiniZinc.Net;

public static class Prelude
{
    public static DirectoryInfo JoinDir(this DirectoryInfo di, string path) =>
        new(Path.Combine(di.FullName, path));

    public static DirectoryInfo JoinDir(this string path, string a) =>
        new(Path.Combine(path, a));
    
    public static string JoinPath(this string path, string a) =>
        Path.Combine(path, a);

    public static FileInfo JoinFile(this string path, string a) =>
        new(Path.Combine(path, a));
    
    public static FileInfo JoinFile(this DirectoryInfo path, string a) =>
        new(Path.Combine(path.FullName, a));
    
    public static FileInfo ToFile(this string path) => new (path);
    
    public static DirectoryInfo ToDirectory(this string path) => new (path);
}
