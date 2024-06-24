using LibMiniZinc.Tests;
using MiniZinc.Parser;

namespace Make;

public sealed class ClientAnySolutionTestsBuilder : ClientTestsBuilder
{
    public ClientAnySolutionTestsBuilder(TestSpec spec)
        : base("ClientAnySolutionTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            MakeTest(info);
        }
    }

    void MakeTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        Var("solution", "await Solve(model, options, SolveStatus.Satisfied, SolveStatus.Optimal)");
        var dzns = info
            .Solutions?.Select(sol => sol.Dzn)
            .Where(dzn => dzn is not null)
            .Select(dzn => dzn!);
        if (dzns is null)
            return;
        WriteLn("string expected = \"\";");
        foreach (var dzn in dzns)
        {
            var result = Parser.ParseDataString(dzn);
            if (!result.Ok)
                continue;
            var expected = result.Data.Write(WriteOptions.Minimal);
            // WriteLn($"expected = \"{expected}\";");
            // using (If($"solution.DataString!.Equals(expected)"))
            //     Return();
        }
        Call("Assert.Fail", "\"Solution did not match any of the expected results\"");
    }
}
