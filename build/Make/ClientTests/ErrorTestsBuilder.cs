namespace Make;

using LibMiniZinc.Tests;

public sealed class ErrorTestsBuilder : ClientTestsBuilder
{
    public ErrorTestsBuilder(TestSpec spec)
        : base("ErrorTests", spec)
    {
        using (
            Function(
                "async Task TestError",
                "string path",
                "string solver",
                "string? errorMessage",
                "string? errorRegex",
                "List<string> args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            using (ForEach("var warn in model.Warnings"))
                WriteMessage("warn");
            Var("options", "SolveOptions.Create(solverId:solver)");
            WriteLn("options = options.AddArgs(args);");
            NewLine();
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeFalse();");
            NewLine();
            using (If("errorRegex is not null"))
                WriteLn("result.Error.Should().MatchRegex(errorRegex);");
            NewLine();
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
            Declare("string", "errorMessage", $"\"{err}\"");
        else
            Declare("string?", "errorMessage", null);

        if (info.ErrorRegex is { } regex)
            Declare("string", "errorRegex", $"\"{regex.Replace("\\", "")}\"");
        else
            Declare("string?", "errorRegex", null);

        WriteLn("await TestError(path, solver, errorMessage, errorRegex, args);");
    }
}
