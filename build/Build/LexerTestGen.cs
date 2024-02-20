namespace Build;

using System.Text;
using LibMiniZinc.Tests;

public sealed class LexerTestGen : CodeBuilder
{
    public readonly TestSpec Spec;

    public LexerTestGen(TestSpec spec)
    {
        Spec = spec;
    }

    public string Generate()
    {
        WriteLn("namespace MiniZinc.Net.Test;");
        WriteLn("using Xunit;");
        Block("public sealed class LexerTests");

        foreach (var @case in Spec.TestCases)
        {
            var path = @case.Path;
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("\\", "_");
            testName = $"test_{testName}";
            Newline();
            WriteLn("[Fact]");
            using var _ = Block($"public void {testName}");
        }

        var code = ToString();
        return code;
    }
}
