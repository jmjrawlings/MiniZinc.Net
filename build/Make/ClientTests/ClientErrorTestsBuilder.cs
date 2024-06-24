namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientErrorTestsBuilder : ClientTestsBuilder
{
    public ClientErrorTestsBuilder(TestSpec spec)
        : base("ClientErrorTests", spec)
    {
        using (
            Function(
                "async Task Test",
                "string path",
                "string solver",
                "string? errorMessage",
                "string? errorRegex",
                "params string[] args"
            )
        )
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
            WriteLn("options = options.AddArgs(args);");
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeFalse();");
            using (If("errorRegex is not null"))
                WriteLn("result.Error.Should().MatchRegex(errorRegex);");
            using (ElseIf("errorMessage is not null"))
                WriteLn("result.Error.Should().Be(errorMessage);");
        }

        foreach (var testCase in spec.TestCases)
        {
            var err = testCase.Type switch
            {
                TestType.Error => true,
                TestType.AssertionError => true,
                TestType.EvaluationError => true,
                TestType.SyntaxError => true,
                TestType.TypeError => true,
                TestType.MiniZincError => true,
                _ => false
            };

            if (!err)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            MakeTest(info);
        }
    }

    void MakeTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        if (info.ErrorMessage is { } err)
            Declare("string?", "errorMessage", $"\"{err}\"");
        else
            Declare("string?", "errorMessage", null);

        if (info.ErrorRegex is { } regex)
            Declare("string?", "errorRegex", $"\"{regex.Replace("\\", "")}\"");
        else
            Declare("string?", "errorRegex", null);

        WriteLn("await Test(path, solver, errorMessage, errorRegex);");
    }
}
