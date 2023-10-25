using System.Reflection;

namespace MiniZinc.Tests;

using System;
using System.IO;

public class TestSuite
{
    private static FileInfo? _solutionFile;
    public static FileInfo SolutionFile => _solutionFile ??= GetSolutionFile();
    
    public static DirectoryInfo ProjectDir => SolutionFile.Directory;
    
    public static DirectoryInfo TestDir => ProjectDir.JoinDir("tests");
    
    public static DirectoryInfo SourceDir => ProjectDir.JoinDir("src");

    public static DirectoryInfo LibMiniZincDir => TestDir.JoinDir("libminizinc");
    
    private static FileInfo GetSolutionFile()
    {
        var assembly = Assembly.GetExecutingAssembly().Location.ToFile();
        var sln = assembly.DirectoryName.JoinFile("MiniZinc.Net.sln");
        while (!sln.Exists)
        {
            var dir = sln.Directory.Parent;
            sln = dir.JoinFile(sln.Name);
        }

        return sln;
    }
}