using System.CommandLine;
using CommunityToolkit.Diagnostics;
using Make;
using MiniZinc.Build;
using MiniZinc.Command;
using MiniZinc.Tests;
using Cmd = MiniZinc.Command.Command;
using Command = System.CommandLine.Command;

var root = new RootCommand("MiniZinc.NET build options");

Add(
    "--clone-libminizinc-tests",
    "Clone the test suite from libminizinc",
    async () =>
    {
        Console.WriteLine("Cloning libminiznc tests");
        var url = $"https://github.com/MiniZinc/libminizinc.git";
        var cloneDir = Environment
            .CurrentDirectory.ToDirectory()
            .JoinDir("libminiznc")
            .CreateOrClear();

        async Task Git(params string[] args)
        {
            var cmd = new Cmd("git", args);
            cmd.WorkingDirectory = cloneDir.FullName;
            Console.WriteLine(cmd);
            var result = await cmd.Run();
            Guard.IsEqualTo((int)result.Status, (int)ProcessStatus.Ok);
        }

        await Git("init");
        await Git("remote", "add", "origin", url);
        await Git("sparse-checkout", "set", "tests/spec");
        await Git("fetch", "origin", "master");
        await Git("checkout", "master");

        var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
        var targetDir = Directory.CreateTempSubdirectory();
        Console.WriteLine($"Copying tests to {targetDir}");
        sourceDir.CopyContentsTo(targetDir);
    }
);

// Add(
//     "--make-parser-tests",
//     "Generate parser tests",
//     async () =>
//     {
//         var spec = LoadSpec();
//         var builder = new ParserTestBuilder();
//         var source = builder.Build(spec);
//         var file = Repo
//             .TestDir.JoinDir("MiniZinc.IntegrationTests")
//             .JoinFile($"{builder.ClassName}.cs");
//         await File.WriteAllTextAsync(file.FullName, source);
//     }
// );
//
Add(
    "--make-client-tests",
    "Generate client tests",
    async () =>
    {
        Console.WriteLine("Parsing libminiznc test suite");
        FileInfo input = Repo.TestSpecYaml;
        Console.WriteLine($"Parsing {input.FullName}");
        TestSpec spec = YamlParser.ParseTestSpecFile(input);
        var testsBySuite = spec.TestCases.GroupBy(t => t.Suite).ToArray();
        foreach (var kv in testsBySuite)
        {
            var suite = kv.Key;
            var tests = kv.ToArray();
            var testClass = $"{suite}IntegrationTests";
            var testFile = Repo.IntegrationTestsDir.JoinFile($"{testClass}.cs");
            var testSource = ClientTestsBuilder.Build(tests, testClass);
            File.WriteAllText(testFile.FullName, testSource);
        }
    }
);

var result = await root.InvokeAsync(args);
return result;
void Add(string name, string desc, Func<Task> handler)
{
    var command = new Command(name: name, description: desc);
    command.SetHandler(handler);
    root.AddCommand(command);
}
