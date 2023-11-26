using System.Text;

namespace MiniZinc.Build;

using System.Collections;
using static Prelude;

public sealed class TestSpec : IEnumerable<TestSuite>
{
    public readonly FileInfo File;
    private readonly List<TestSuite> _suites;
    private readonly List<TestCase> _cases;
    private readonly StringBuilder _sb;

    public static TestSpec Create(FileInfo file)
    {
        return new TestSpec(file);
    }

    private TestSpec(FileInfo file)
    {
        _sb = new StringBuilder();
        _suites = new();
        _cases = new();
        File = file;
        var document = Yaml.ParseFile(File)!;
        foreach (var node in document)
            CreateTestSuite(node);
    }

    private void CreateTestSuite(YamlNode node)
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

        foreach (var fi in suite.IncludeFiles)
            CreateTestCase(suite, fi);

        _suites.Add(suite);
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
            Console.Write(c);
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
            yield return doc;
        }
    }

    private void CreateTestCase(TestSuite suite, FileInfo file)
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
            var a = 1;
        }
    }

    public IEnumerator<TestSuite> GetEnumerator() => _suites.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
