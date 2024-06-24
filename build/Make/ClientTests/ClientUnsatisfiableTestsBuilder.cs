namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientUnsatisfiableTestsBuilder : ClientTestsBuilder
{
    public ClientUnsatisfiableTestsBuilder(TestSpec spec)
        : base("ClientUnsatisfiableTests", spec)
    {
        using (Function("async Task Test", "string path", "string solver", "string[]? args"))
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            using (ForEach("var warn in model.Warnings"))
            {
                WriteMessage("warn");
            }
            Var("options", "SolveOptions.Create(solverId:solver)");
            using (If("args is not null"))
                WriteLn("options = options.AddArgs(args);");
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeFalse();");
            WriteLn("result.Status.Should().Be(SolveStatus.Unsatisfiable);");
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Unsatisfiable)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        WriteLn("Test(path, solver);");
    }
}
