namespace MiniZinc.Tests;

public static class FileExtensions
{
    public static string JoinPath(this string path, params string[] a)
    {
        string p = path;
        foreach (var s in a)
            p = Path.Combine(p, s);

        return p;
    }

    public static DirectoryInfo JoinDir(this DirectoryInfo di, params string[] path) =>
        di.FullName.JoinPath(path).ToDirectory();

    public static FileInfo JoinFile(this DirectoryInfo di, params string[] path) =>
        di.FullName.JoinPath(path).ToFile();

    public static FileInfo ToFile(this string path) => new(path);

    public static DirectoryInfo ToDirectory(this string path) => new(path);

    public static T EnsureExists<T>(this T fsi)
        where T : FileSystemInfo
    {
        if (!fsi.Exists)
            throw new Exception($"{fsi} does not exist");
        return fsi;
    }

    public static DirectoryInfo CreateOrClear(this DirectoryInfo di)
    {
        if (!di.Exists)
        {
            di.Create();
            return di;
        }

        foreach (var file in di.EnumerateFiles())
            file.Delete();

        foreach (var dir in di.EnumerateDirectories())
            dir.Delete(true);
        return di;
    }

    public static DirectoryInfo CopyContentsTo(this DirectoryInfo di, DirectoryInfo dj)
    {
        dj.CreateOrClear();
        foreach (var file in di.EnumerateFiles())
        {
            var target = dj.JoinFile(file.Name);
            file.CopyTo(target.FullName);
        }

        var dirs = di.GetDirectories();
        foreach (var dir in dirs)
        {
            var target = dj.JoinDir(dir.Name);
            dir.CopyContentsTo(target);
        }

        return dj;
    }

    public static DirectoryInfo CopyContentsTo(this DirectoryInfo di, string path)
    {
        var dj = new DirectoryInfo(path);
        return di.CopyContentsTo(dj);
    }

    public static string RelativeTo(this FileSystemInfo fsi, string path)
    {
        var uri = Path.GetRelativePath(path, fsi.FullName);
        return uri;
    }

    public static string RelativeTo(this FileSystemInfo fsi, FileSystemInfo other) =>
        fsi.RelativeTo(other.FullName);
}
