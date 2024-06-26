namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientUnsatisfiableTestsBuilder : ClientTestsBuilder
{
    public ClientUnsatisfiableTestsBuilder(TestSpec spec)
        : base("ClientUnsatisfiableTests", spec)
    {
        using (Function("async Task Test", "string path", "string solver", "params string[] args"))
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
        Write("await Test(path, solver");
        AppendArgs(info.ExtraArgs);
        AppendLn(");");
    }
}
