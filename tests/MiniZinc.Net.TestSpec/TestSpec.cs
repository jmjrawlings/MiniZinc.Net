using System.Text;

namespace MiniZinc.Build;

using System.Collections;

public sealed class TestSpec : IEnumerable<TestSuite>
{
    public readonly FileInfo File;
    private readonly List<TestSuite> _suites;
    private readonly HashSet<string> _files;
    private readonly StringBuilder _sb;
    public readonly FileInfo SpecFile;
    public readonly DirectoryInfo SpecDir;

    public IEnumerable<string> Files => _files;
    public IEnumerable<TestCase> TestCases => _suites.SelectMany(s => s.TestCases);

    public static TestSpec Parse(FileInfo file)
    {
        return new TestSpec(file);
    }

    private TestSpec(FileInfo file)
    {
        SpecFile = file;
        SpecDir = file.Directory!;
        _sb = new StringBuilder();
        _suites = new();
        _files = new HashSet<string>();
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
            // Options = node["options"]
        };

        suite.Solvers.AddRange(node["solvers"].Select(x => x.String!));
        suite.IncludeGlobs.AddRange(node["includes"].Select(x => x.String!));
        suite
            .IncludeFiles
            .AddRange(
                suite
                    .IncludeGlobs
                    .SelectMany(glob => SpecDir.EnumerateFiles(glob, SearchOption.AllDirectories))
                    .Select(x => x.FullName)
            );

        foreach (var testFile in suite.IncludeFiles)
        {
            foreach (var testCase in CreateTestCases(suite, testFile))
            {
                suite.TestCases.Add(testCase);
                _files.Add(testFile);
            }
        }

        return suite;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    private IEnumerable<YamlNode> ParseTestCaseYaml(string path)
    {
        var file = new FileInfo(path);
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

    private IEnumerable<TestCase> CreateTestCases(TestSuite suite, string path)
    {
        foreach (var node in ParseTestCaseYaml(path))
        {
            var testName = path;
            var testCase = new TestCase
            {
                TestName = testName,
                TestPath = path,
                Includes = node["includes"].Select(x => x.String!).ToList(),
                Solvers = node["solvers"].Select(x => x.String!).ToList(),
                // SolveOptions = node["options"]
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
        var result = new TestResult { };
        return result;
    }

    public IEnumerator<TestSuite> GetEnumerator() => _suites.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
