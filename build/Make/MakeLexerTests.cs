﻿namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeLexerTests : CodeBuilder
{
    private MakeLexerTests(TestSpec spec)
    {
        var paths = spec.TestCases.Select(c => c.Path).Distinct().ToList();
        Generate(paths);
    }

    void Generate(IEnumerable<string> files)
    {
        WriteLn(
            """
            /*
            THIS FILE WAS GENERATED BY THE FOLLOWING COMMAND

            dotnet run --project ./build/Make/Make.csproj --make-lexer-tests
            */
            """
        );
        Block("public sealed class LexerIntegrationTests");
        using (Block("private void Test(string path)"))
        {
            Var("lexer", "Lexer.LexFile(path)");
            Var("tokens", "lexer.ToArray()");
        }

        foreach (var path in files)
        {
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("/", "_");
            testName = testName.Replace("-", "_");
            testName = $"test_lex_{testName}";
            Newline();
            WriteLn($"[Fact(DisplayName = \"{path}\")]");
            using (Block($"public void {testName}()"))
            {
                Var("path", $"\"{path}\"");
                WriteLn("Test(path);");
            }
        }
    }

    public static async Task Run()
    {
        var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        var source = new MakeLexerTests(spec).ToString();
        var file = Projects.ParserTests.Dir.JoinFile("LexerIntegrationTests.cs");
        await File.WriteAllTextAsync(file.FullName, source);
    }
}
