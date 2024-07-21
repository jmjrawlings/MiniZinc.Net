﻿namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Build;

public sealed class MakeParserTests : TestBuilder
{
    private MakeParserTests(TestSpec spec)
        : base("ParserIntegrationTests", spec)
    {
        var files = new HashSet<string>();
        foreach (var @case in spec.TestCases)
        {
            if (@case.Type is TestType.Error or TestType.SyntaxError)
                continue;

            files.Add(@case.Path);
        }

        Make(files);
    }

    void Make(IEnumerable<string> files)
    {
        using (BlockComment())
        {
            WriteLn("<auto-generated>");
            WriteLn("This file was generated by the following command:");
            WriteLn("dotnet run --project ./build/Make/Make.csproj --make-parser-tests");
            Write("</auto-generated>");
        }

        Block($"public sealed class {ClassName}");
        using (Block("private void TestParse(string path)"))
        {
            Var("result", "Parser.ParseModelFile(path, out var model)");
            WriteLn("result.Ok.Should().BeTrue();");
        }

        foreach (var path in files)
        {
            var testName = path.Replace(".mzn", "");
            testName = testName.Replace("/", "_");
            testName = testName.Replace("-", "_");
            testName = testName.Replace(".", "");
            testName = $"test_parse_{testName}";
            NewLine();
            Attribute("Fact", $"DisplayName=\"{path}\"");
            using (Block($"public void {testName}()"))
            {
                Var("path", $"\"{path}\"");
                WriteLn("TestParse(path);");
            }
        }
    }

    public static async Task Run()
    {
        // var spec = TestSpec.FromJsonFile(Repo.TestSpecJson);
        // var builder = new MakeParserTests(spec);
        // builder.WriteTo(Projects.ParserTests.Dir);
        await Task.CompletedTask;
    }
}
