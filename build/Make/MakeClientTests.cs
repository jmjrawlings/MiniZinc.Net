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

        builder = new AllSolutionsTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new OptimiseTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new ErrorTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new UnsatisfiableTestsBuilder(spec);
        builder.WriteTo(dir);

        builder = new AnySolutionTestsBuilder(spec);
        builder.WriteTo(dir);
    }
}
