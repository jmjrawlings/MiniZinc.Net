﻿using System.Text.Json;
using System.Text.Json.Nodes;
using LibMiniZinc.Tests;
using Make;
using MiniZinc.Build;

public abstract class ClientTestsBuilder : CodeBuilder
{
    public readonly TestSpec Spec;
    public readonly string ClassName;

    protected ClientTestsBuilder(string className, TestSpec spec)
    {
        ClassName = className;
        Spec = spec;
        WriteLn(
            """
            /*
            THIS FILE WAS GENERATED BY THE FOLLOWING COMMAND

            dotnet run --project ./build/Make/Make.csproj --make-client-tests
            */
            """
        );

        Block($"public class {className} : IClassFixture<ClientFixture>");
        WriteLn("private readonly MiniZincClient MiniZinc;");
        WriteLn("private readonly ITestOutputHelper _output;");

        using (Function($"public {className}", "ClientFixture fixture", "ITestOutputHelper output"))
        {
            WriteLn("MiniZinc = fixture.Client;");
            WriteLn("_output = output;");
        }
    }

    protected void WriteMessage(object? msg)
    {
        WriteLn($"_output.WriteLine({msg?.ToString() ?? string.Empty});");
    }

    protected void WriteSection() => WriteMessage("new string('-',80)");

    protected IDisposable WriteTestHeader(TestCaseInfo info)
    {
        Attribute($"Theory(DisplayName=\"{info.Path}\")");
        foreach (var (solver, enabled) in info.Solvers)
        {
            if (enabled)
                Attribute($"InlineData(\"{solver}\")");
            else
                Attribute($"InlineData(\"{solver}\", Skip=\"Solver not supported\")");
        }
        var block = Block($"public async Task {info.Name}(string solver)");
        Var("path", $"\"{info.Path}\"");
        return block;
    }

    protected TestCaseInfo? GetTestInfo(TestCase testCase)
    {
        var testName = testCase.Path.Replace(".mzn", "");
        testName = testName.Replace("/", "_");
        testName = testName.Replace("-", "_");
        testName = $"test_solve_{testName}";
        if (testCase.Sequence > 1)
            testName = $"{testName}_{testCase.Sequence}";

        var testPath = testCase.Path;
        var testDir = Path.GetDirectoryName(testPath)!;
        var info = new TestCaseInfo
        {
            Path = testPath,
            Dir = testDir,
            Name = testName,
            Signature = $"async Task {testName}(string solver)",
            Options = testCase.Options,
            Sequence = testCase.Sequence,
            Solutions = testCase.Solutions,
            InputFiles = testCase.InputFiles,
            ErrorMessage = testCase.ErrorMessage,
            ErrorRegex = testCase.ErrorRegex,
            CheckAgainstSolvers = testCase.CheckAgainstSolvers,
            Type = testCase.Type,
            OutputFiles = testCase.OutputFiles
        };

        if (testCase.Solvers is null)
            return null;

        if (testCase.Options is JsonObject opts)
        {
            foreach (var kv in opts)
            {
                var key = kv.Key;
                var val = kv.Value;
                var kind = val.GetValueKind();
                if (!key.StartsWith('-'))
                    key = $"--{key}";

                string arg;
                if (kind is JsonValueKind.True)
                {
                    arg = key;
                }
                else
                {
                    var value = val.ToString();
                    if (value.Contains(' '))
                        arg = $"{key} \\\"{value}\\\"";
                    else
                        arg = $"{key} {value}";
                }
                info.ExtraArgs.Add(arg);
            }
        }

        if (testCase.InputFiles is { } extraFiles)
        {
            foreach (var extraFile in extraFiles)
            {
                var extraPath = Path.Combine(testDir!, extraFile);
                extraPath = extraPath.Replace('\\', '/');
                info.ExtraArgs.Add($"--data \\\"{extraPath}\\\"");
            }
        }

        foreach (var solver in testCase.Solvers)
        {
            info.Solvers.Add(
                solver switch
                {
                    "cbc" => ("coin-bc", true),
                    "scip" => ("scip", false),
                    "gurobi" => ("gurobi", false),
                    _ => (solver, true)
                }
            );
        }

        return info;
    }

    public void WriteTo(DirectoryInfo directory)
    {
        var file = directory.JoinFile($"{ClassName}.cs");
        var source = ToString();
        File.WriteAllText(file.FullName, source);
    }
}