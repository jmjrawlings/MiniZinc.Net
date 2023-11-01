namespace MiniZinc.Tests;

public sealed class TestCase
{
    public readonly string TestName;

    public readonly string TestPath;

    public readonly FileInfo TestFile;

    public readonly List<FileInfo> Includes;

    public readonly List<string> Solvers;

    public readonly TestSuite TestSuite;

    public readonly List<TestResult> TestResults;

    public TestCase(TestSuite suite, FileInfo file)
    {
        TestSuite = suite;
        TestResults = new List<TestResult>();
        Solvers = new List<string>();
        Includes = new List<FileInfo>();
        TestResults = new List<TestResult>();
        TestPath = file.FullName;
        TestName = Path.GetFileNameWithoutExtension(file.Name);
        TestFile = file;
    }

    public override string ToString() => $"<Test \"{TestFile.Name}\">";
}
