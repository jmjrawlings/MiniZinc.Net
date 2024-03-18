namespace Build;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class GenerateParserTests : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly IEnumerable<string> Files;

    public GenerateParserTests(TestSpec spec)
    {
        Spec = spec;
        Files = spec.TestCases.Select(c => c.Path).Distinct().ToList();
    }

    string Generate()
    {
        Block("public sealed class ParserTests");
        using (Block("private void Test(string mzn)"))
        {
            Var("lexer", "Lexer.LexFile(path)");
            Var("parser", "new Parser(lexer)");
            WriteLn("parser.ParseModel(out var model);");
            using (If("parser._error is { } err"))
                WriteLn("Assert.Fail(err);");
        }

        foreach (var path in Files)
        {
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("\\", "_");
            testName = testName.Replace("-", "_");
            testName = $"test_{testName}";
            Newline();
            WriteLn("[Fact]");
            using (Block($"public void {testName}()"))
            {
                Var("path", $"@\"{path}\"");
                WriteLn("Test(path);");
            }
        }

        var code = ToString();
        return code;
    }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var gen = new GenerateParserTests(spec);
        var source = gen.Generate();
        var file = Projects.ParserTests.Dir.JoinFile("ParserIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
