namespace MiniZinc.Build;

using System.Reflection;

public static class Repo
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

    private static FileInfo? _solutionFile;
    public static FileInfo SolutionFile
    {
        get
        {
            if (_solutionFile is not null)
                return _solutionFile;

            var assembly = Assembly.GetExecutingAssembly().Location.ToFile();
            var sln = assembly.Directory!.JoinFile("MiniZinc.Net.sln");
            while (!sln.Exists)
            {
                var dir = sln.Directory!.Parent;
                sln = dir!.JoinFile(sln.Name);
            }

            _solutionFile = sln;
            return sln;
        }
    }
    public static DirectoryInfo SolutionDir => SolutionFile.Directory!;
    public static DirectoryInfo SourceDir => SolutionDir.JoinDir("src");
    public static DirectoryInfo TestDir => SolutionDir.JoinDir("test");
    public static DirectoryInfo BuildDir => SolutionDir.JoinDir("build");
    public static DirectoryInfo LibMiniZincDir => TestDir.JoinDir("libminizinc");
    public static DirectoryInfo TestSpecDir => LibMiniZincDir;
    public static FileInfo TestSpecYaml => TestSpecDir.JoinFile("suites.yml");
    public static FileInfo TestSpecJson => TestDir.JoinFile("suites.json");
}
