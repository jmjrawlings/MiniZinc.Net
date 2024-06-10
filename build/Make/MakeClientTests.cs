namespace Make;

using System.Text.Json;
using System.Text.Json.Nodes;
using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeClientTests : CodeBuilder
{
    private MakeClientTests(TestSpec spec)
    {
        Block("public class ClientIntegrationTests : IClassFixture<ClientFixture>");
        WriteLn("private readonly MiniZincClient _client;");
        WriteLn("private readonly ITestOutputHelper _output;");
        using (
            Block("public ClientIntegrationTests(ClientFixture fixture, ITestOutputHelper output)")
        )
        {
            WriteLn("_client = fixture.Client;");
            WriteLn("_output = output;");
        }
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

        var solverIds = new List<string>();
        foreach (var solver in solvers)
        {
            var solverId = solver switch
            {
                "cbc" => "coin-bc",
                "gurobi" => null,
                _ => solver
            };

            if (solverId is not null)
                solverIds.Add(solverId);
        }

        if (solverIds.Count == 0)
            return;

        List<string> extraArgs = new List<string>();
        if (testCase.Options is JsonObject opts)
        {
            foreach (var kv in opts)
            {
                var key = kv.Key;
                var val = kv.Value;
                var kind = val.GetValueKind();
                if (!key.StartsWith('-'))
                    key = $"--{key}";

                if (kind is JsonValueKind.True)
                    extraArgs.Add($"{key}");
                else
                    extraArgs.Add($"{key} {val}");
            }
        }

        Attribute($"Theory(DisplayName=\"{path}\")");
        foreach (var solver in solverIds)
            Attribute($"InlineData(\"{solver}\")");

        var block = Block($"public async void {testName}(string solver)");
        Var("path", $"\"{path}\"");
        Var("timeout", "TimeSpan.FromSeconds(30)");
        Var("options", "SolveOptions.Create(solverId:solver, timeout: timeout)");
        foreach (var arg in extraArgs)
            WriteLn($"options = options.AddArgs(\"{arg}\");");

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

    private void MakeErrorTest(string testName, TestCase testCase) { }

    private void MakeUnsatisfiableTest(string testName, TestCase testCase) { }

    private void MakeOutputTest(string testName, TestCase testCase) { }

    private void MakeAllSolutionsTest(string testName, TestCase testCase)
    {
        Var("model", "Model.FromFile(path)");
        Var("solutions", "new List<Solution>()");
        using (Block("await foreach (var solution in _client.Solutions(model,options))"))
        {
            WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
        }
    }

    private void MakeAnySolutionTest(string testName, TestCase testCase)
    {
        Var("model", "Model.FromFile(path)");
        Var("solution", "await _client.Solve(model, options)");
        WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
    }

    private void MakeSatisfyTest(string testName, TestCase testCase)
    {
        Var("model", "Model.FromFile(path)");
        Var("solution", "await _client.Solve(model,options)");
        WriteLn("solution.Status.Should().Be(SolveStatus.Satisfied);");
    }

    private void MakeCompileTest(string testName, TestCase testCase) { }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var generator = new MakeClientTests(spec);
        var source = generator.ToString();
        var file = Projects.ClientTests.Dir.JoinFile("ClientIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
