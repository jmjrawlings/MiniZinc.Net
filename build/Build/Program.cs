using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic.FileIO;
using MiniZinc.Build;
using static MiniZinc.Build.Prelude;
using static System.Console;

var cloneTestsCommand = new Command(
    name: "--clone-tests",
    description: "Clone the test suite from libminizinc"
);

var generateTestsCommand = new Command(
    name: "--generate-test-db",
    description: "Generate test cases from the test spec"
);

cloneTestsCommand.SetHandler(CloneTests);
generateTestsCommand.SetHandler(GenerateTestDB);

var rootCommand = new RootCommand("MiniZinc.NET build options");
rootCommand.AddCommand(cloneTestsCommand);
rootCommand.AddCommand(generateTestsCommand);
var result = await rootCommand.InvokeAsync(args);
return result;

async Task CloneTests()
{
    var url = $"{LibMiniZincUrl}.git";
    var libDir = LibMiniZincDir.CreateOrClear();
    var cloneDir = Environment
        .CurrentDirectory
        .JoinPath(Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
        .ToDirectory()
        .CreateOrClear();

    async Task<int> Git(params object[] args)
    {
        var arg = string.Join(" ", args);
        Console.WriteLine(arg);
        var exit = await ProcessUtils.Exec("git", arg, workDir: cloneDir.FullName);
        return exit.ExitCode ?? 0;
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

async Task GenerateTestDB()
{
    var spec = TestSpec.Parse(TestSpecFile);
    var json = JsonSerializer.Serialize(spec);
    var file = BuildDir.JoinFile("tests.json");
    await using var stream = file.OpenWrite();
    await using var writer = new StreamWriter(stream);
    await writer.WriteAsync(json);
}
