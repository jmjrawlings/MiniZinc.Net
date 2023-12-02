using System.Text;

namespace MiniZinc.Build;

using System.Collections;
using static Prelude;

public sealed class TestSpec : IEnumerable<TestSuite>
{
    public readonly FileInfo File;
    private readonly List<TestSuite> _suites;
    private readonly List<TestCase> _cases;
    private readonly HashSet<FileInfo> _files;
    private readonly StringBuilder _sb;

    public IEnumerable<FileInfo> Files => _files;
    public IEnumerable<TestCase> TestCases => _cases;

    public static TestSpec Parse(FileInfo file)
    {
        return new TestSpec(file);
    }

    private TestSpec(FileInfo file)
    {
        _sb = new StringBuilder();
        _suites = new();
        _cases = new();
        _files = new HashSet<FileInfo>();
        File = file;
        var document = Yaml.ParseFile(File)!;
        foreach (var node in document)
        {
            var suite = CreateTestSuite(node);
            _suites.Add(suite);
        }
    }

    private TestSuite CreateTestSuite(YamlNode node)
    {
        var suite = new TestSuite
        {
            Name = node.Key!,
            Strict = node["strict"].Bool,
            Options = node["options"]
        };

        suite.Solvers.AddRange(node["solvers"].Select(x => x.String!));
        suite.IncludeGlobs.AddRange(node["includes"].Select(x => x.String!));
        suite
            .IncludeFiles
            .AddRange(
                suite
                    .IncludeGlobs
                    .SelectMany(
                        glob => LibMiniZincDir.EnumerateFiles(glob, SearchOption.AllDirectories)
                    )
            );

        foreach (var testFile in suite.IncludeFiles)
        {
            foreach (var testCase in CreateTestCases(suite, testFile))
            {
                suite.TestCases.Add(testCase);
                _files.Add(testFile);
                _cases.Add(testCase);
            }
        }

        return suite;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    private IEnumerable<YamlNode> ParseTestCaseYaml(FileInfo file)
    {
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);
        _sb.Clear();

        const char EOF = '\uffff';
        char c = EOF;
        int i = -1;
        void Read()
        {
            i++;
            c = (char)reader.Read();
        }

        bool Skip(char c)
        {
            if (reader.Peek() != c)
                return false;

            Read();
            return true;
        }

        if (!Skip('/'))
            yield break;

        preamble:
        Read();
        if (c is '\n' or '*' or '-')
            goto preamble;
        if (char.IsWhiteSpace(c))
            goto preamble;

        contents:
        _sb.Append(c);
        if (reader.EndOfStream)
            goto fin;
        Read();

        if (c is '*')
            if (Skip('*'))
                if (Skip('*'))
                    if (Skip('/'))
                        goto fin;
        goto contents;

        fin:

        var yamlString = _sb.ToString();
        var testStrings = yamlString.Split(
            "---",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );
        foreach (var s in testStrings)
        {
            var doc = Yaml.ParseString(s);
            if (doc is not null)
                yield return doc;
        }
    }

    private IEnumerable<TestCase> CreateTestCases(TestSuite suite, FileInfo file)
    {
        foreach (var node in ParseTestCaseYaml(file))
        {
            var testName = file.FullName;
            var testCase = new TestCase
            {
                TestSuite = suite,
                TestName = testName,
                TestPath = file.FullName,
                TestFile = file,
                Includes = node["includes"].Select(x => x.String!.ToFile()).ToList(),
                Solvers = node["solvers"].Select(x => x.String!).ToList(),
                SolveOptions = node["options"]
            };
            var check = node["check_against"];
            var extra = node["extra"];
            var expected = node["expected"];
            foreach (var enode in expected)
            {
                var result = CreateTestResult(enode);
                testCase.Results.Add(result);
            }

            yield return testCase;
        }
    }

    private TestResult CreateTestResult(YamlNode node)
    {
        var sol = node["solution"];
        var result = new TestResult { Expected = sol };
        return result;
    }

    public IEnumerator<TestSuite> GetEnumerator() => _suites.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
