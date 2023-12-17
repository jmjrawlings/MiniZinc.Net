namespace MiniZinc.Net.Tests;

using System.Text;

internal sealed class TestSpecParser
{
    public readonly FileInfo File;
    public readonly DirectoryInfo Dir;
    private readonly StringBuilder _sb;
    public readonly TestSpec Spec;

    internal TestSpecParser(FileInfo file)
    {
        File = file;
        Dir = file.Directory!;
        _sb = new StringBuilder();
        List<TestSuite> suites = new();
        var document = Yaml.ParseFile(file)!;
        foreach (var node in document)
        {
            var suite = ParseTestSuite(node);
            suites.Add(suite);
        }

        Spec = new TestSpec { FileName = file.Name, TestSuites = suites };
    }

    private TestSuite ParseTestSuite(YamlNode node)
    {
        var suite = new TestSuite
        {
            Name = node.Key!,
            Strict = node["strict"].Bool,
            // Options = node["options"]
        };

        suite.Solvers.AddRange(node["solvers"].Select(x => x.String!));
        suite.IncludeGlobs.AddRange(node["includes"].Select(x => x.String!));

        foreach (var glob in suite.IncludeGlobs)
        {
            foreach (var file in Dir.EnumerateFiles(glob, SearchOption.AllDirectories))
            {
                var path = Path.GetRelativePath(Dir.FullName, file.FullName);

                foreach (var testCase in ParseTestCases(path))
                {
                    suite.TestCases.Add(testCase);
                }
            }
        }

        return suite;
    }

    /// <summary>
    /// Parse yaml contained within the test file comments
    /// </summary>
    private IEnumerable<YamlNode> ParseTestCase(string relativePath)
    {
        var path = Path.Join(Dir.FullName, relativePath);
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

    private IEnumerable<TestCase> ParseTestCases(string path)
    {
        foreach (var node in ParseTestCase(path))
        {
            var testName = Path.GetFileNameWithoutExtension(path);
            var testCase = new TestCase
            {
                TestName = testName,
                TestPath = path,
                Solvers = node["solvers"].Select(x => x.String!).ToList(),
                // SolveOptions = node["options"]
            };
            var check = node["check_against"];
            var extra = node["extra"];
            var expected = node["expected"];
            foreach (var enode in expected)
            {
                var result = ParseTestResult(enode);
                testCase.Results.Add(result);
            }

            yield return testCase;
        }
    }

    private TestResult ParseTestResult(YamlNode node)
    {
        var sol = node["solution"];
        var result = new TestResult { };
        return result;
    }
}
