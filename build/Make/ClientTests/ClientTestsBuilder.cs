﻿namespace Make;

using System.Text;
using LibMiniZinc.Tests;

public sealed class ClientTestsBuilder : TestBuilder
{
    public ClientTestsBuilder(string className, TestSpec spec)
        : base(className, spec)
    {
        using (BlockComment())
        {
            WriteLn("<auto-generated>");
            WriteLn("This file has been auto generated by the following command:");
            WriteLn("dotnet run --project ./build/Make/Make.csproj --make-client-tests");
            Write("</auto-generated>");
        }

        WriteLn("#nullable enable");
        NewLine();
        Block($"public class {className} : ClientTest");
        NewLine();

        using (
            Block(
                $"public {className}(ITestOutputHelper output, ClientFixture fixture) : base(output, fixture)"
            )
        ) { }

        foreach (var testCase in spec.TestCases)
        {
            if (GetTestInfo(testCase) is not { } info)
                continue;

            var ok = testCase.Type switch
            {
                TestType.Satisfy => true,
                TestType.Optimise => true,
                TestType.AnySolution => true,
                TestType.AllSolutions => true,
                TestType.Unsatisfiable => true,
                _ => false
            };
            if (!ok)
                continue;

            WriteTest(info);
        }
    }

    protected void WriteMessage(object? msg = null)
    {
        if (msg is null)
            Call("_output.WriteLine", "\"\"");
        else
            Call("_output.WriteLine", msg.ToString()!);
    }

    protected void WriteSection() => WriteMessage("new string('-',80)");

    protected void WriteTest(TestCaseInfo info)
    {
        IDisposable block;
        List<string> args = [$"path: \"{info.Path}\""];
        if (info.Solvers.Count > 1)
        {
            Attribute("Theory", $"DisplayName=\"{info.Path}\"");
            foreach (var (solver, enabled) in info.Solvers)
            {
                if (enabled)
                    Attribute("InlineData", $"\"{solver}\"");
                else
                    Attribute($"InlineData", $"\"{solver}\", Skip=\"Solver not supported\"");
            }

            block = Function($"public async Task {info.Name}", "string solver");
            args.Add("solver: solver");
        }
        else
        {
            var (solver, enabled) = info.Solvers[0];
            if (enabled)
                Attribute("Fact", $"DisplayName=\"{info.Path}\"");
            else
                Attribute("Fact", $"DisplayName=\"{info.Path}\"", "Skip=\"Solver not supported\"");

            block = Function($"public async Task {info.Name}");
            args.Add($"solver: {Quote(solver)}");
        }

        var ab = new StringBuilder();

        if (info.Solutions is { Count: > 0 } solutions)
        {
            ab.Append("solutions: [");
            int i = 0;
            foreach (var sol in solutions)
            {
                ab.Append(TripleQuote(sol));
                if (++i < solutions.Count)
                    ab.Append(',');
            }
            ab.Append(']');
            args.Add(ab.ToString());
        }

        ab.Clear();
        if (info.ExtraArgs is { Count: > 0 } extraArgs)
        {
            ab.Append("args: [");
            int i = 0;
            foreach (var arg in extraArgs)
            {
                ab.Append(FormatArg(arg));
                if (++i < extraArgs.Count)
                    ab.Append(',');
            }

            ab.Append(']');
            args.Add(ab.ToString());
        }
        ab.Clear();

        if (info.ErrorMessage is { } err)
            args.Add($"error: \"{err}\"");
        else if (info.ErrorRegex is { } regex)
        {
            var rgx = regex.Replace("\\", "");
            args.Add($"error: \"{rgx}\"");
        }

        if (info.Type is TestType.AllSolutions)
            args.Add($"allSolutions: true");

        switch (info.Type)
        {
            case TestType.Compile:
                break;
            case TestType.Satisfy:
                args.Add("statuses: [SolveStatus.Satisfied,SolveStatus.Optimal]");
                break;
            case TestType.AnySolution:
                args.Add("statuses: [SolveStatus.Satisfied,SolveStatus.Optimal]");
                break;
            case TestType.AllSolutions:
                args.Add("statuses: [SolveStatus.Satisfied,SolveStatus.Optimal]");
                break;
            case TestType.Optimise:
                args.Add("statuses: [SolveStatus.Optimal]");
                break;
            case TestType.OutputModel:
                break;
            case TestType.Unsatisfiable:
                args.Add("statuses: [SolveStatus.Unsatisfiable]");
                break;
            case TestType.Error:
                break;
            case TestType.AssertionError:
                break;
            case TestType.EvaluationError:
                break;
            case TestType.MiniZincError:
                break;
            case TestType.TypeError:
                break;
            case TestType.SyntaxError:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        WriteLn("await Test(");
        using (Indent())
        {
            int n = 0;
            foreach (var arg in args)
            {
                if (++n < args.Count)
                    WriteLn($"{arg},");
                else
                    WriteLn(arg);
            }
        }
        WriteLn(");");
        block.Dispose();
    }

    private string FormatDzn(string s)
    {
        var z = s.Replace("\n", "\\n");
        z = z.Replace("\"", "\\\"");
        z = $"\"{z}\"";
        return z;
    }

    private string FormatArg(string s)
    {
        // Some extra options are passed in as quoted strings
        if (s.StartsWith('"'))
            return s;

        var z = Quote(s);
        return z;
    }
}
