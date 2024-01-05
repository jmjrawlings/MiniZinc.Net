namespace MiniZinc.Net.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CommunityToolkit.Diagnostics;
using static Prelude;

public sealed class YamlTests : TestBase
{
    [Theory]
    [InlineData("unit\\compilation\\aggregation.mzn")]
    void Parse_Test_case(string path)
    {
        var file = LibMiniZincDir.JoinFile(path);
        foreach (var yaml in TestSpec.ParseTestCaseYaml(file))
        {
            var json = Yaml.ParseString<JsonObject>(yaml);
            var tcase = TestSpec.ParseTestCase(json!);
            var a = 2;
        }
    }

    [Fact]
    void Parse_Test_Suite_Yaml_As_Json()
    {
        var json = Yaml.ParseFile(TestSpecYaml);
        Guard.IsNotNull(json);
    }

    [Fact]
    void Parse_Test_Spec_From_Yaml()
    {
        var spec = TestSpec.ParseTestSpecFromYaml(TestSpecYaml);
        var a = spec;
    }

    [Fact]
    void Test_Spec_Parse_Integration()
    {
        var spec1 = TestSpec.ParseTestSpecFromYaml(TestSpecYaml);
        var file = Json.SerializeToFile(spec1, TestSpecJson);
        var spec2 = TestSpec.ParseTestSpecFromJson(TestSpecJson);
        var a = 2;
    }

    [Fact]
    void Parse_Test_Result()
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

    [Fact]
    void Parse_Sets_And_Ranges()
    {
        var yaml = """
        !Test
        expected:
        - !Result
            solution: !Solution
                alldisj_avsi: [!Range 6..7, !Range 3..4, !!set {5, 8}, !Range 1..2]
""";
        var map = Yaml.ParseString<JsonObject>(yaml);
        // map["__tag__"]?.GetValue<string>().Should().Be(Yaml.TEST);
        var exp = map["expected"]!.AsArray()[0]!;
        var sol = exp["solution"]!;
        var a = sol["alldisj_avsi"]!.AsArray();
        var b = 1;
    }

    [Fact]
    void Serialize_Test_Result()
    {
        var result = new TestCase { Type = TestType.Solution };
        var converter = new JsonStringEnumConverter();
        var options = new JsonSerializerOptions();
        options.Converters.Add(converter);
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        var json = JsonSerializer.Serialize(result, options);
        json.Should().BeEquivalentTo("""{"Type":"Solution"}""");
    }

    public YamlTests(LoggingFixture logging, ITestOutputHelper output)
        : base(logging, output) { }
}
