namespace MiniZinc.Net;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;

public enum CommandStatus
{
    Started = 0,
    StdOut = 1,
    StdErr = 2,
    Success = 3,
    Failure = 4,
}

public readonly record struct CommandOutput
{
    public required string Command { get; init; }
    public required int ProcessId { get; init; }
    public required string Message { get; init; }
    public required DateTimeOffset StartTime { get; init; }
    public required DateTimeOffset TimeStamp { get; init; }
    public required TimeSpan Elapsed { get; init; }
    public required CommandStatus Status { get; init; }
}

public readonly record struct CommandResult
{
    public required string Command { get; init; }
    public required DateTimeOffset StartTime { get; init; }
    public required DateTimeOffset EndTime { get; init; }
    public required TimeSpan Duration { get; init; }
    public required string StdOut { get; init; }
    public required string StdErr { get; init; }
    public required int ExitCode { get; init; }
    public required bool IsError { get; init; }

    public void EnsureSuccess()
    {
        if (!IsError)
            return;

        var msg = $"The command \"{Command}\" exited with code {ExitCode}:\n \"{StdErr}\"";
        throw new Exception(msg);
    }
}

public readonly record struct Arg
{
    public readonly string? Flag;
    public readonly string? Value;
    public readonly bool Eq;
    public readonly string String;

    public Arg(string? flag, bool? eq, string? value)
    {
        Flag = flag;
        Eq = eq ?? false;
        Value = value;
        if (value is null)
            String = flag!;
        else if (flag is null)
            String = value;
        else if (Eq)
            String = $"{Flag}={Value}";
        else
            String = $"{Flag} {Value}";
    }

    public override string ToString() => String;
}

public readonly partial record struct Command
{
    public readonly string Exe;
    public readonly Arg[]? Args;
    public readonly string String;
    public readonly DirectoryInfo? WorkingDir;
    private const string ArgsPattern = """(-{1,2}[a-zA-Z]\w*)?\s*(=)?\s*("[^"]*"|[^\s]+)?""";

    private Command(string exe, Arg[]? args = null, DirectoryInfo? workingDir = null)
    {
        Exe = exe;
        Args = args;
        WorkingDir = workingDir;
        if (args is null)
        {
            String = exe;
        }
        else
        {
            String = $"{exe} {string.Join(" ", args)}";
        }
    }

    public Command WithWorkingDirectory(string path) =>
        WithWorkingDirectory(new DirectoryInfo(path));

    public Command WithArgs(params string[]? args) =>
        Create(Exe, args).WithWorkingDirectory(WorkingDir);

    public Command WithWorkingDirectory(DirectoryInfo? dir) => new(Exe, Args, dir);

    public static Command Create(string exe, params string[]? args)
    {
        Guard.IsNotNullOrWhiteSpace(exe);
        Command cmd;
        if (args is null)
        {
            cmd = new Command(exe);
        }
        else
        {
            var arr = ParseArgs(args).ToArray();
            cmd = new Command(exe, arr);
        }

        return cmd;
    }

    public async Task<CommandResult> Run()
    {
        var runner = new CommandRunner(this);
        var result = await runner.Run();
        return result;
    }

    public async IAsyncEnumerable<CommandOutput> Stream(ILogger? logger = null)
    {
        var stream = new CommandStreamer(this, logger);
        await foreach (var msg in stream.Stream())
        {
            yield return msg;
        }
    }

    public static async Task<CommandResult> Run(string exe, params string[]? args)
    {
        var cmd = Create(exe, args);
        var result = await cmd.Run();
        return result;
    }

    private static Arg ParseArg(Match m)
    {
        Group g;
        g = m.Groups[1];
        var flag = g.Success ? g.Value : null;

        g = m.Groups[2];
        var eq = g.Success;

        g = m.Groups[3];
        var value = g.Success ? g.Value : null;
        var arg = new Arg(flag, eq, value);
        return arg;
    }

    public static Arg? ParseArg(string s)
    {
        var m = ArgsRegex().Match(s);
        if (!m.Success)
            return null;
        var arg = ParseArg(m);
        return arg;
    }

    public static IEnumerable<Arg> ParseArgs(params string[] args)
    {
        var regex = ArgsRegex();
        foreach (var arg in args)
        {
            var matches = regex.Matches(arg);
            foreach (Match m in matches)
            {
                if (m.Length <= 0)
                    continue;

                var a = ParseArg(m);
                yield return a;
            }
        }
    }

    [GeneratedRegex(ArgsPattern)]
    private static partial Regex ArgsRegex();
}
