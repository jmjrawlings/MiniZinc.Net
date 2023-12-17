namespace MiniZinc.Net.Tests;

using System.Reflection;

public static class Prelude
{
    private static FileInfo? _solutionFile;

    public static FileInfo SolutionFile => _solutionFile ??= GetSolutionFile();

    public static DirectoryInfo ProjectDir => SolutionFile.Directory!;

    public static DirectoryInfo TestDir => ProjectDir.JoinDir("tests");

    public static FileInfo TestSpecJson => TestDir.JoinFile("tests.json");

    public static DirectoryInfo SourceDir => ProjectDir.JoinDir("src");

    public static DirectoryInfo LibMiniZincDir => ProjectDir.JoinDir("libminizinc");

    public static FileInfo TestSuiteFile => LibMiniZincDir.JoinFile("suites.yml");

    private static FileInfo GetSolutionFile()
    {
        var assembly = Assembly.GetExecutingAssembly().Location.ToFile();
        var sln = assembly.Directory!.JoinFile("MiniZinc.Net.sln");
        while (!sln.Exists)
        {
            var dir = sln.Directory!.Parent;
            sln = dir!.JoinFile(sln.Name);
        }

        return sln;
    }

    public static V? TryGet<K, V>(this IDictionary<K, V> dict, K key)
        where K : notnull
    {
        if (dict.TryGetValue(key, out var item))
            return item;
        return default;
    }
}
