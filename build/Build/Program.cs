﻿using System;
using System.CommandLine;
using Build;
using LibMiniZinc.Tests;
using MiniZinc.Net;
using Command = MiniZinc.Net.Command;

var rootCommand = new RootCommand("MiniZinc.NET build options");

void AddCommand(string name, string desc, Func<Task> handler)
{
    var command = new System.CommandLine.Command(name: name, description: desc);
    command.SetHandler(handler);
    rootCommand.AddCommand(command);
}

AddCommand(
    "--clone-libminizinc-tests",
    "Clone the test suite from libminizinc",
    CloneLibMiniZincTests
);
AddCommand(
    "--parse-libminizinc-tests",
    "Generate test cases from the test spec",
    Parse_LibMiniZinc_Tests
);

AddCommand("--generate-lexer-tests", "Generate lexer tests", Generate_LibMiniZinc_Lexer_Tests);

var result = await rootCommand.InvokeAsync(args);
return result;

async Task CloneLibMiniZincTests()
{
    var url = $"https://github.com/MiniZinc/libminizinc.git";
    var libDir = Repo.LibMiniZincDir.CreateOrClear();
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

async Task Parse_LibMiniZinc_Tests()
{
    var spec = Spec.ParseYaml(Repo.TestSpecYaml);
    TestSpec.ToJsonFile(spec, Repo.TestSpecJson);
}

async Task Generate_LibMiniZinc_Lexer_Tests()
{
    var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
    LibMiniZincLexerTests.Generate(spec);
    var a = 1;
}
