namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientSatisfyTestsBuilder : ClientTestsBuilder
{
    public ClientSatisfyTestsBuilder(TestSpec spec)
        : base("SatisfyTests", spec)
    {
        using (
            Function(
                "async Task TestSatisfy",
                "string path",
                "string solver",
                "List<(string, bool)> solutions",
                "List<string> args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            NewLine();
            Var("model", "Model.FromFile(path)");
            Call("model.Satisfy");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver)");
            WriteLn("options = options.AddArgs(args);");
            NewLine();
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.Status.Should().Be(SolveStatus.Satisfied);");
            using (ForEach("var (dzn,output) in solutions"))
            {
                Var("expected", "Parser.ParseDataString(dzn, out var data);");
                WriteLn("expected.Ok.Should().BeTrue();");
                using (If("!result.Data.Equals(data)"))
                    WriteLn("Assert.Fail(\"\");");
            }
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Satisfy)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        WriteLn("await TestSatisfy(path, solver, solutions, args);");
    }
}
