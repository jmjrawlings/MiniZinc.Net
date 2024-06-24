using LibMiniZinc.Tests;
using MiniZinc.Parser;

namespace Make;

public sealed class ClientAnySolutionTestsBuilder : ClientTestsBuilder
{
    public ClientAnySolutionTestsBuilder(string name, IEnumerable<TestCase> testCases) : base(name, testCases)
    {
        foreach (var testCase in testCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;
            _testCases.Add(testCase);
        }
        
        foreach (var testCase in _testCases)
            MakeTest(testCase);
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