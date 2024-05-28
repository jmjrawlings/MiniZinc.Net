namespace Build;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeLexerTests : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly IEnumerable<string> Files;

    private MakeLexerTests(TestSpec spec)
    {
        Spec = spec;
        Files = spec.TestCases.Select(c => c.Path).Distinct().ToList();
    }

    string Generate()
    {
        Block("public sealed class LexerIntegrationTests");
        using (Block("private void Test(string path)"))
        {
            Var("lexer", "Lexer.LexFile(path)");
            Var("tokens", "lexer.ToArray()");
        }

        foreach (var path in Files)
        {
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("/", "_");
            testName = testName.Replace("-", "_");
            testName = $"test_{testName}";
            Newline();
            WriteLn($"[Fact(DisplayName = \"{path}\")]");
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
        var gen = new MakeLexerTests(spec);
        var source = gen.Generate();
        var file = Projects.ParserTests.Dir.JoinFile("LexerIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
