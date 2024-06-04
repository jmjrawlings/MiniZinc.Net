namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Build;
using static Console;

public static class ParseLibMiniZincTests
{
    public static Task Run()
    {
        WriteLine("Parsing libminiznc test suite");
        FileInfo input = Repo.TestSpecYaml;
        FileInfo output = Repo.TestSpecJson;

        WriteLine($"Parsing {input.FullName}");
        var spec = Spec.ParseYaml(input);

        WriteLine($"Writing to {output.FullName}");
        TestSpec.ToJsonFile(spec, output);
        return Task.CompletedTask;
    }
}
