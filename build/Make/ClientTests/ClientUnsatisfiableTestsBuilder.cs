using LibMiniZinc.Tests;

namespace Make;

public sealed class ClientUnsatisfiableTestsBuilder : ClientTestsBuilder
{
    public ClientUnsatisfiableTestsBuilder(string name, IEnumerable<TestCase> testCases) : base(name, testCases)
    {
        using (Function("async Task Test", "string path", "string solverId", "params string args"))
        {
            Write(
                """
                Write(path);
                WriteSection();
                var model = Model.FromFile(path);
                Write(model.SourceText);
                WriteSection();
                foreach (var warn in model.Warnings)
                    WriteWarning(warn);
                var options = SolveOptions.Create(solverId:solver);
                options = options.AddArgs(args);
                var solution = await _client.Solve(model, options);
                solution.Status.Should.Be(SolveStatus.Unsatisfiable);
                """
            );
        }
        
        foreach (var testCase in testCases)
        {
            if (testCase.Type is not TestType.Unsatisfiable)
                continue;
            
            if (testCase.Sequence > 1)
                var testName = 
                
            
            
                    MakeTest(testCase);
        }
    }
    
    void MakeTest(TestCase testCase)
    {
        using (Function($"async Task {testCase.} "))
        
            WriteLn("Test(path, solver);");
    }
}