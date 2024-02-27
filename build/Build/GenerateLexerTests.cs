using MiniZinc.Build;

namespace Build;

using System.Text;
using LibMiniZinc.Tests;

public sealed class GenerateLexerTests : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly IEnumerable<string> Files;

    public GenerateLexerTests(TestSpec spec)
    {
        Spec = spec;
        Files = spec.TestCases.Select(c => c.Path).Distinct().ToList();
    }

    string Generate()
    {
        Block("public sealed class LexerTests");

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
                Var("lexer", "Lexer.LexFile(path)");
                Var("tokens", "lexer.ToArray()");
            }
        }

        var code = ToString();
        return code;
    }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var gen = new GenerateLexerTests(spec);
        var source = gen.Generate();
        var file = Projects.ParserTests.Dir.JoinFile("LexerIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
