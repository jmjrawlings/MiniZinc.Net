namespace Build;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeSolverTests : CodeBuilder
{
    private MakeSolverTests(TestSpec spec)
    {
        Block("public class SolverIntegrationTests");
        Newline();
        WriteLn("private readonly MiniZincClient _client;");
        Newline();
        using (
            Block("public SolverIntegrationTests(ClientFixture fixture, ITestOutputHelper output)")
        )
        {
            WriteLn("_client = fixture.Client;");
        }
        Newline();

        var files = spec.TestCases.GroupBy(c => c.Path);
        foreach (var group in files)
        {
            int i = 1;
            var g = group.ToList();
            foreach (var testCase in group)
            {
                testCase.Sequence = i++;
                MakeTest(testCase);
            }
        }
    }

    /// <summary>
    /// Generate an xunit test from the given TestSpec test case
    /// </summary>
    void MakeTest(TestCase testCase)
    {
        var testName = testCase.Path.Replace(".mzn", "");
        testName = testName.Replace("/", "_");
        testName = testName.Replace("-", "_");
        testName = $"test_{testName}";
        if (testCase.Sequence > 1)
            testName = $"{testName}_case_{testCase.Sequence}";

        var path = testCase.Path;
        if (testCase.Solvers is not { } solvers)
            return;

        Attribute($"Theory(DisplayName=\"{path}\")");
        foreach (var solver in solvers)
            Attribute($"InlineData(\"{solver}\")");

        var block = Block($"public async void {testName}(string solver)");
        Var("path", $"\"{path}\"");
        switch (testCase.Type)
        {
            case TestType.Compile:
                MakeCompileTest(testName, testCase);
                break;
            case TestType.Satisfy:
                MakeSatisfyTest(testName, testCase);
                break;
            case TestType.AnySolution:
                MakeAnySolutionTest(testName, testCase);
                break;
            case TestType.AllSolutions:
                MakeAllSolutionsTest(testName, testCase);
                break;
            case TestType.OutputModel:
                MakeOutputTest(testName, testCase);
                break;
            case TestType.Unsatisfiable:
                MakeUnsatisfiableTest(testName, testCase);
                break;
            case TestType.Error:
                MakeErrorTest(testName, testCase);
                break;
        }
        block.Dispose();
        Newline();
    }

    private void MakeErrorTest(string testName, TestCase testCase)
    {
        throw new NotImplementedException();
    }

    private void MakeUnsatisfiableTest(string testName, TestCase testCase)
    {
        throw new NotImplementedException();
    }

    private void MakeOutputTest(string testName, TestCase testCase)
    {
        throw new NotImplementedException();
    }

    private void MakeAllSolutionsTest(string testName, TestCase testCase)
    {
        Var("solve", "_client.SolveModelFile(path, solver)");
        Var("solution", "await solve.Solution()");
        WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
    }

    private void MakeAnySolutionTest(string testName, TestCase testCase)
    {
        Var("solve", "_client.SolveModelFile(path, solver)");
        Var("solution", "await solve.Solution()");
        WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
    }

    private void MakeSatisfyTest(string testName, TestCase testCase)
    {
        Var("solve", "_client.SolveModelFile(path, solver)");
        Var("solution", "await solve.Solution()");
        WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
    }

    private void MakeCompileTest(string testName, TestCase testCase)
    {
        throw new NotImplementedException();
    }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var generator = new MakeSolverTests(spec);
        var source = generator.ToString();
        var file = Projects.ClientTests.Dir.JoinFile("ParserIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
