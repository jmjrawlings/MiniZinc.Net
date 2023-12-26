using System.Text.Json.Nodes;

namespace MiniZinc.Net.Tests;

public sealed class YamlTests : TestBase
{
    [Fact]
    public void Parse_Test_Suite_Yaml()
    {
        var file = "suites.yml".ToFile();
        var json = Yaml.ParseFile(file);
    }

    [Fact]
    public void Parse_Test_Spec()
    {
        var file = "suites.yml".ToFile();
        var spec = TestSpec.ParseYaml(file);
        var a = spec;
    }

    [Fact]
    public void Parse_Test_Result()
    {
        var str = """
            !Test
            expected:
            - !Result
              solution: !Solution
                a:
                - [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
                - [0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0]
                - [0, 1, 0, 0, 0, 0, 0, 0, 2, 0, 0]
                - [0, 2, 0, 0, 0, 1, 0, 0, 3, 0, 0]
                - [0, 0, 0, 1, 0, 2, 0, 0, 0, 0, 0]
                - [0, 0, 0, 2, 0, 0, 0, 0, 0, 1, 0]
                - [0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0]
                - [0, 1, 0, 0, 0, 0, 1, 2, 3, 4, 0]
                - [0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0]
                - [0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 0]
                - [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
                col_sums: [4, 0, 3, 2, 2, 2, 1, 4, 2]
                row_sums: [2, 2, 3, 2, 2, 1, 5, 1, 2]
            """;
        var map = Yaml.ParseString<JsonObject>(str);
        // map["__tag__"]?.GetValue<string>().Should().Be(Yaml.TEST);
        var exp = map["expected"]!.AsArray()[0]!;
        var sol = exp["solution"]!;
        var a = sol["a"]!;
    }

    public YamlTests(LoggingFixture logging, ITestOutputHelper output)
        : base(logging, output) { }
}
