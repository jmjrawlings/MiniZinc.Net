﻿using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualBasic.FileIO;
using MiniZinc.Build;
using MiniZinc.Net.Build;
using MiniZinc.Tests;
using static MiniZinc.Net.Build.Prelude;
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
generateTestsCommand.SetHandler(GenerateTestDatabase);

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

async Task GenerateTestDatabase()
{
    var yaml = TestSpecFile;
    var json = TestDir.JoinFile("tests.json");
    var spec = TestSpec.ParseYaml(TestSpecFile);
    await TestSpec.WriteJson(spec, json);
}
