namespace Build;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public static class ParseLibMiniZincTests
{
    public static async Task Run()
    {
        var spec = Spec.ParseYaml(Repo.TestSpecYaml);
        TestSpec.ToJsonFile(spec, Repo.TestSpecJson);
    }
}
