namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientAllSolutionsTestsBuilder : ClientTestsBuilder
{
    public ClientAllSolutionsTestsBuilder(TestSpec spec)
        : base("ClientAllSolutionsTests", spec)
    {
        using (
            Function(
                "async Task Test",
                "string path",
                "string solver",
                "List<(string,bool)> solutions",
                "params string[] args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            NewLine();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver)");
            Assign("options", "options.AddArgs(args)");
            NewLine();
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.DataString.Should().NotBeNull();");
            WriteLn("result.Data.Should().NotBeNull();");
            NewLine();

            using (ForEach("var (dzn,output) in solutions"))
            {
                Var("expected", "Parser.ParseDataString(dzn);");
                WriteLn("expected.Ok.Should().BeTrue();");
                WriteLn("result.Data.Should().Be(expected.Data);");
            }
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        Write("await Test(path, solver, solutions");
        AppendArgs(info.ExtraArgs);
        AppendLn(");");
    }
}
