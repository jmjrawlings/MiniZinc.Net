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
        var builder = new ClientTestsBuilder("ClientIntegrationTests", spec);
        builder.WriteTo(dir);
        await Task.CompletedTask;
    }
}
