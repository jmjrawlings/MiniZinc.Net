namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

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
                "List<string> validSolutions"
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
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.DataString.Should().NotBeNull();");
            WriteLn("result.Data.Should().NotBeNull();");
            Var("solution", "result.Data!");

            using (ForEach("var dzn in validSolutions"))
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
        WriteLn("var validSolutions = new List<string> {");

        using (Indent())
        {
            foreach (var sol in validSolutions)
            {
                if (sol.Dzn is not { } dzn)
                    continue;
                Write(Quote(dzn));
                Append(',');
                NewLine();
            }
        }
        WriteLn("};");
        NewLine();
        WriteLn("await Test(path, solver, validSolutions);");
    }
}
