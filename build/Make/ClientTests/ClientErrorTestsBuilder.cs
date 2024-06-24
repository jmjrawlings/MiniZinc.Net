using LibMiniZinc.Tests;

namespace Make;

public sealed class ClientAllSolutionsTestsBuilder : ClientTestsBuilder
{
    public ClientAllSolutionsTestsBuilder(string name, IEnumerable<TestCase> testCases) : base(name, testCases)
    {
        foreach (var testCase in testCases)
        {
            if (testCase.Type is not TestType.AllSolutions)
                continue;
            
            if (testCase.Solvers is not { } solvers)
                continue;
            
            MakeTest(testCase, solvers);
        }
    }
    
    void MakeTest(TestCase testCase, IReadOnlyList<string> solvers)
    {
        Var("solution", "await Solve(model, options, SolveStatus.Satisfied)");
    }
}