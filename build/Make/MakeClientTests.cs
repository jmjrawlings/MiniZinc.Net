namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public static class MakeClientTests
{
    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var files = spec.TestCases.GroupBy(c => c.Path);
        foreach (var group in files)
        {
            int i = 1;
            var g = group.ToList();
            foreach (var testCase in group)
            {
                testCase.Sequence = i++;
            }
        }

        var dir = Projects.ClientTests.Dir;
        ClientTestsBuilder builder;

        builder = new ClientSatisfyTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ClientAllSolutionsTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ClientOptimiseTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ClientErrorTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ClientUnsatisfiableTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ClientAnySolutionTestsBuilder(spec);
        builder.WriteTo(dir);
    }
}
