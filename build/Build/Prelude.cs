namespace MiniZinc.Net.Build;

using System.Reflection;
using MiniZinc.Net.Build;

public static class Prelude
{
    public const string LibMiniZincUrl = "https://github.com/MiniZinc/libminizinc";

    private static FileInfo? _solutionFile;

    public static FileInfo SolutionFile => _solutionFile ??= GetSolutionFile();

    public static DirectoryInfo ProjectDir => SolutionFile.Directory!;

    public static DirectoryInfo TestDir => ProjectDir.JoinDir("tests");

    public static DirectoryInfo SourceDir => ProjectDir.JoinDir("src");

    public static DirectoryInfo BuildDir => ProjectDir.JoinDir("build");

    public static DirectoryInfo LibMiniZincDir => ProjectDir.JoinDir("libminizinc");

    public static FileInfo TestSpecYaml => LibMiniZincDir.JoinFile("suites.yml");

    public static FileInfo TestSpecJson => LibMiniZincDir.JoinFile("tests.json");

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
}
