using System.CommandLine;
using MiniZinc.Net;
using MiniZinc.Net.Build;
using MiniZinc.Net.Tests;
using static MiniZinc.Net.Build.Prelude;
using CliCommand = System.CommandLine.Command;
using Command = MiniZinc.Net.Command;

var cloneTestsCommand = new CliCommand(
    name: "--clone-tests",
    description: "Clone the test suite from libminizinc"
);

var generateTestsCommand = new CliCommand(
    name: "--generate-test-db",
    description: "Generate test cases from the test spec"
);

cloneTestsCommand.SetHandler(CloneLibMiniZincTestSpec);
generateTestsCommand.SetHandler(GenerateTestDatabase);

var rootCommand = new RootCommand("MiniZinc.NET build options");
rootCommand.AddCommand(cloneTestsCommand);
rootCommand.AddCommand(generateTestsCommand);
var result = await rootCommand.InvokeAsync(args);
return result;

async Task CloneLibMiniZincTestSpec()
{
    var url = $"{LibMiniZincUrl}.git";
    var libDir = LibMiniZincDir.CreateOrClear();
    var cloneDir = Environment
        .CurrentDirectory
        .JoinPath(Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
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

async Task GenerateTestDatabase()
{
    var yaml = TestSpecFile;
    var json = TestDir.JoinFile("tests.json");
    var spec = TestSpec.ParseYaml(TestSpecFile);
    await TestSpec.WriteJson(spec, json);
}
