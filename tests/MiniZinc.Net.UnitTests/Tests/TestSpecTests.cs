namespace MiniZinc.Net.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;
using static FileExtensions;

public sealed class TestSpecTests : TestBase
{
    [Theory]
    [InlineData("unit\\compilation\\par_arg_out_of_bounds.mzn")]
    void Parse_TestCase_From_String(string path)
    {
        var file = LibMiniZincDir.JoinFile(path);
        foreach (var yaml in TestSpec.ParseTestCaseYaml(file))
        {
            var json = Yaml.ParseString<JsonObject>(yaml);
            var tcase = TestSpec.ParseTestCase(path, json!);
            var a = 2;
        }
    }

    [Fact]
    void Parse_TestSpec_From_Yaml()
    {
        var cwd = Directory.GetCurrentDirectory().ToDirectory();
        var spec1 = TestSpec.ParseYaml(TestSpecYaml);
        // var dst = cwd.JoinFile("suites.json");
        var dst = TestSpecJson;
        Json.SerializeToFile(spec1, dst);
        var spec2 = TestSpec.ParseJson(dst);
        var a = 2;
    }

    [Fact]
    void Parse_Test_Results_From_String()
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
        var exp = map["expected"]!.AsArray()[0]!;
        var sol = exp["solution"]!;
        var a = sol["a"]!;
    }

    [Fact]
    void Parse_Sets_And_Ranges_From_String()
    {
        var yaml = """
        !Test
        expected:
        - !Result
            solution: !Solution
                alldisj_avsi: [!Range 6..7, !Range 3..4, !!set {5, 8}, !Range 1..2]
""";
        var map = Yaml.ParseString<JsonObject>(yaml)!;
        var exp = map["expected"]!.AsArray()[0]!;
        var sol = exp["solution"]!;
        var a = sol["alldisj_avsi"]!.AsArray();
        var b = 1;
    }

    [Fact]
    void Serialize_TestCase()
    {
        var result = new TestCase { Type = TestType.AnySolution };
        var options = Json.SerializerOptions;
        var json = JsonSerializer.Serialize(result, options);
        json.Should().BeEquivalentTo("""{"type":"any_solution"}""");
    }

    public TestSpecTests(LoggingFixture logging, ITestOutputHelper output)
        : base(logging, output) { }
}
