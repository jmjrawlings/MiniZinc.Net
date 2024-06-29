namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

public sealed class OptimiseTestsBuilder : ClientTestsBuilder
{
    public OptimiseTestsBuilder(TestSpec spec)
        : base("OptimiseTests", spec)
    {
        using (
            Function(
                "async Task TestOptimise",
                "string path",
                "string solver",
                "List<(string, bool)> solutions",
                "List<string> args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver).AddArgs(args)");
            WriteLn("options = options.AddArgs(args);");
            NewLine();
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.Status.Should().Be(SolveStatus.Optimal);");
            using (ForEach("var (dzn, isOutput) in solutions"))
            {
                Var("expected", "Parser.ParseDataString(dzn, out var data);");
                WriteLn("expected.Ok.Should().BeTrue();");
                using (If("!result.Data.Equals(data)"))
                    WriteLn("Assert.Fail(\"\");");
            }
            NewLine();
        }
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Optimise)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        WriteLn("await TestOptimise(path, solver, solutions, args);");
    }
}
