namespace Build;

using System.Text;
using LibMiniZinc.Tests;

public sealed class LexerTestGen : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly IEnumerable<string> Files;

    public LexerTestGen(TestSpec spec)
    {
        Spec = spec;
        Files = spec.TestCases.Select(c => c.Path).Distinct().ToList();
    }

    public string Generate()
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
}
