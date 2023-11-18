namespace MiniZinc.Tests;

using System.Collections;

using static Prelude;

public class TestSuites : IEnumerable<object[]>
{
    readonly List<object[]> _suites = new();

    public TestSuites()
    {
        foreach (var suite in Load())
        {
            _suites.Add(new object[] { suite });
        }
    }

    public static IEnumerable<TestSuite> Load(FileInfo? file = null)
    {
        var yaml = Yaml.ParseFile(file ?? TestSuiteFile)!;
        var suites = new List<TestSuite>();
        foreach (var node in yaml)
        {
            var suite = new TestSuite
            {
                Name = node.Key!,
                Strict = node["strict"].Bool,
                Options = node["options"],
                Solvers = node["solvers"].Select(x => x.String!).ToList(),
                IncludeGlobs = node["includes"].Select(x => x.String!).ToList(),
            };
            suite.IncludeFiles = suite.IncludeGlobs
                .SelectMany(
                    glob => LibMiniZincDir.EnumerateFiles(glob, SearchOption.AllDirectories)
                )
                .ToList();
            suites.Add(suite);

            foreach (var fi in suite.IncludeFiles)
            {
                Load(suite, fi);
            }
        }

        return suites;
    }

    static void Load(TestSuite suite, FileInfo file)
    {
        // Test case yaml are stored as comments in each minizinc model
        var token = Lexer.LexFile(file, lexBlockComments: true).First();
        if (token.Kind != TokenKind.BlockComment)
            return;

        var comment = token.String!;
        var n = comment.Length;
        int i;
        int j;

        for (i = 0; i < n; i++)
        {
            if (comment[i] is '*' or '\n')
                continue;
            break;
        }

        for (j = n - 1; j >= 0; j--)
        {
            if (comment[j] is '*' or '\n')
                continue;
            break;
        }

        var yamlString = comment.AsSpan(i, j - i + 1).ToString();
        var testStrings = yamlString.Split(
            "---",
            StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries
        );

        foreach (var testString in testStrings)
        {
            var yaml = Yaml.ParseString(testString);
            if (yaml is null)
            {
                Console.Write("Could not parse {0} yaml from {1}", file.FullName, testString);
                continue;
            }

            var testName = file.FullName;
            var testCase = new TestCase
            {
                TestSuite = suite,
                TestName = testName,
                TestPath = file.FullName,
                TestFile = file,
                Includes = yaml["includes"].Select(x => x.String!.ToFile()).ToList(),
                Solvers = yaml["solvers"].Select(x => x.String!).ToList(),
                SolveOptions = yaml["options"]
            };
            var check = yaml["check_against"];
            var extra = yaml["extra"];
            var expected = yaml["expected"];
            var a = 1;
        }
    }

    public IEnumerator<object[]> GetEnumerator() => _suites.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
