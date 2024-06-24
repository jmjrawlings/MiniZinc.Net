namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

public sealed class ClientOptimiseTestsBuilder : ClientTestsBuilder
{
    public ClientOptimiseTestsBuilder(string name, IEnumerable<TestCase> testCases)
        : base(name)
    {
        foreach (var testCase in testCases)
        {
            if (testCase.Type is not TestType.Optimise)
                continue;

            if (testCase.Solvers is not { } solvers)
                continue;

            MakeTest(testCase, solvers);
        }
    }

    void MakeTest(TestCase testCase)
    {
        Var("solution", "await Solve(model, options, SolveStatus.Satisfied, SolveStatus.Optimal)");
        var dzns = testCase
            .Solutions?.Select(sol => sol.Dzn)
            .Where(dzn => dzn is not null)
            .Select(dzn => dzn!);
        if (dzns is null)
            return;
        WriteLn("string expected;");
        foreach (var dzn in dzns)
        {
            var result = Parser.ParseDataString(dzn);
            if (!result.Ok)
                continue;
            var expected = result.Data.Write(WriteOptions.Minimal);
            WriteLn($"expected = \"{expected}\";");
            using (If($"solution.DataString!.Equals(expected)"))
                Return();
        }
        Call("Assert.Fail", "\"Solution did not match any of the expected results\"");
    }
}
