namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientAnySolutionTestsBuilder : ClientTestsBuilder
{
    public ClientAnySolutionTestsBuilder(TestSpec spec)
        : base("ClientAnySolutionTests", spec)
    {
        using (
            Function(
                "async Task Test",
                "string path",
                "string solver",
                "List<string> anySolutions",
                "bool output",
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
            Var("solution", "result.Data!");

            using (ForEach("var dzn in anySolutions"))
            {
                Var("expectedSolution", "Parser.ParseDataString(dzn);");
                WriteLn("expectedSolution.Ok.Should().BeTrue();");
                Var("ok", "true");
                using (ForEach("var assign in expectedSolution.Data.Assignments"))
                {
                    Var("name", "assign.Name");
                    Var("actual", "solution[name]");
                    Var("expected", "assign.Expr");
                    Var("actualDzn", "actual.ToString()");
                    Var("expectedDzn", "expected.ToString()");
                    using (If("!expectedDzn.Equals(actualDzn)"))
                    {
                        Assign("ok", "false");
                        Break();
                    }
                }

                using (If("ok"))
                    Return();
            }
            Call("Assert.Fail", Quote("xd"));
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            if (info.Solutions is not { } validSolutions)
                continue;

            if (validSolutions.Count is 0)
                continue;

            WriteTest(info, validSolutions);
        }
    }

    void WriteTest(TestCaseInfo info, List<TestSolution> validSolutions)
    {
        using var _ = WriteTestHeader(info);
        WriteLn("var solutions = new List<string> {");
        var output = false;
        using (Indent())
        {
            foreach (var sol in validSolutions)
            {
                if (sol.Dzn is { } dzn)
                {
                    Write(sol.Dzn);
                    Append(',');
                    NewLine();
                }
                else if (sol.Ozn is { } ozn)
                {
                    Write(sol.Dzn);
                    Append(',');
                    NewLine();
                    output = true;
                }
            }
        }
        WriteLn("};");
        NewLine();
        Write("await Test(path, solver, solutions, ");
        Append(output ? "true" : "false");
        AppendArgs(info.ExtraArgs);
        AppendLn(");");
    }
}
