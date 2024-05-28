namespace Build;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeParserTests : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly IEnumerable<string> Files;

    private MakeParserTests(TestSpec spec)
    {
        Spec = spec;
        var files = new HashSet<string>();
        foreach (var @case in spec.TestCases)
        {
            if (@case.Type is TestType.Error)
                continue;

            files.Add(@case.Path);
        }

        Files = files.ToList();
    }

    string Make()
    {
        Block("public sealed class ParserIntegrationTests");
        using (Block("private void Test(string path)"))
        {
            Var("lexer", "Lexer.LexFile(path)");
            Var("parser", "new Parser(lexer)");
            WriteLn("parser.ParseModel(out var model);");
            using (If("parser.ErrorString is { } err"))
                WriteLn("Assert.Fail(err);");
        }

        foreach (var path in Files)
        {
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("/", "_");
            testName = testName.Replace("-", "_");
            testName = $"test_{testName}";
            Newline();
            WriteLn($"[Fact(DisplayName=\"{path}\")]");
            using (Block($"public void {testName}()"))
            {
                Var("path", $"\"{path}\"");
                WriteLn("Test(path);");
            }
        }

        var code = ToString();
        return code;
    }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var gen = new MakeParserTests(spec);
        var source = gen.Make();
        var file = Projects.ParserTests.Dir.JoinFile("ParserIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
