namespace MiniZinc.Tests;

using Build;
using Core;
using Shouldly;

public class SpecTests
{
    [Test]
    public async Task TestParseTestSpec()
    {
        var source = FileSystemExtensions.ToFile("./spec/suites.yml");
        var spec = TestParser.ParseTestSpecFile(source);
        var target = Path.Join(Repo.IntegrationTestsDir.FullName, "spec.json");
        spec.TestCases.ShouldNotBeEmpty();
        spec.ToJsonFile(target);
    }

    // [Test]
    // public async Task TestParseTestSpecLimited()
    // {
    //     var source = FileSystemExtensions.ToFile("./spec/test.yml");
    //     var spec = TestParser.ParseTestSpecFile(source);
    //     var target = Path.Join(Repo.IntegrationTestsDir.FullName, "spec.json");
    //     spec.TestCases.ShouldNotBeEmpty();
    //     spec.ToJsonFile(target);
    // }

    [Test]
    [Arguments("spec/examples/packing.mzn")]
    public async Task TestParseTestCase(string path)
    {
        var source = FileSystemExtensions.ToFile(path);
        var yaml = TestParser.GetTestCaseYaml(source).ToList();
        foreach (var tcase in yaml)
        {
            var test = TestParser.ParseTestCase(tcase);
            var b = 2;
        }

        var a = 2;
    }

    // [Test]
    // public async Task test_parse_spec_yaml()
    // {
    //     var yaml = """
    //         !Test
    //         check_against: []
    //         expected:
    //         - !Result
    //           solution: !Solution
    //             x: 1
    //           status: SATISFIED
    //         extra_files: []
    //         markers: []
    //         name: ''
    //         options:
    //           all_solutions: false
    //         solvers:
    //         - gecode
    //         type: solve
    //         """;
    //     var test = TestParser.ParseTestFromString(yaml);
    //     test.ShouldNotBeNull();
    //     test.Options.ShouldNotBeNull();
    // }
}
