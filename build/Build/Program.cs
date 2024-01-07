using System.CommandLine;
using MiniZinc.Build;
using MiniZinc.Net;
using MiniZinc.Net.Build;
using MiniZinc.Net.Tests;
using static MiniZinc.Net.Build.Prelude;
using CliCommand = System.CommandLine.Command;
using Command = MiniZinc.Net.Command;

var cloneTestsCommand = new CliCommand(
    name: "--clone-libminizinc-tests",
    description: "Clone the test suite from libminizinc"
);

var genTestsJsonCommand = new CliCommand(
    name: "--gen-tests-json",
    description: "Generate test cases from the test spec"
);

var genLexerIntegrationTestsCommand = new CliCommand(
    name: "--gen-lexer-tests",
    description: "Generate lexer tests"
);

cloneTestsCommand.SetHandler(CloneLibMiniZincTests);
genTestsJsonCommand.SetHandler(GenerateTestsJson);
genLexerIntegrationTestsCommand.SetHandler(GenerateLexerIntegrationTests);

var rootCommand = new RootCommand("MiniZinc.NET build options");
rootCommand.AddCommand(cloneTestsCommand);
rootCommand.AddCommand(genTestsJsonCommand);
rootCommand.AddCommand(genLexerIntegrationTestsCommand);
var result = await rootCommand.InvokeAsync(args);
return result;

async Task CloneLibMiniZincTests()
{
    var url = $"{LibMiniZincUrl}.git";
    var libDir = LibMiniZincDir.CreateOrClear();
    var cloneDir = Environment
        .CurrentDirectory.JoinPath(Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
        .ToDirectory()
        .CreateOrClear();

    async Task<CommandResult> Git(params string[] args)
    {
        var cmd = Command.Create("git", args).WithWorkingDirectory(cloneDir);
        var result = await cmd.Run();
        result.EnsureSuccess();
        return result;
    }

    await Git("init");
    await Git("remote", "add", "origin", url);
    await Git("sparse-checkout", "set", "tests/spec");
    await Git("fetch", "origin", "master");
    await Git("checkout", "master");

    var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
    var targetDir = libDir;
    sourceDir.CopyContentsTo(targetDir);
    cloneDir.Delete(true);
}

void GenerateTestsJson()
{
    var spec = TestSpec.ParseYaml(TestSpecYaml);
    Json.SerializeToFile(spec, TestSpecJson);
}

void GenerateLexerIntegrationTests()
{
    var spec = Json.DeserializeFromFile<TestSpec>(TestSpecJson);
    var result = LexerTests.Generate(spec);
    var a = 1;
}
