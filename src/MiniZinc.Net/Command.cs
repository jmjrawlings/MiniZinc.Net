using Microsoft.Extensions.Logging;

namespace MiniZinc.Net;

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

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
}

public readonly partial record struct Arg
{
    public readonly string? Flag;
    public readonly string? Value;
    public readonly string? Sep;
    public readonly string String;

    public Arg(string? flag, string? sep, string? value)
    {
        Flag = flag;
        Sep = sep;
        Value = value;
        String = $"{Flag}{Sep}{Value}";
    }

    public override string ToString() => String;

    const string quotedArgPattern = "\"[^\"]*\"";
    const string unquotedArgPattern = """[^\s\"<>|&;]*""";
    const string flagPattern = """-(?:-?[\w|-]+)""";
    const string assignPattern = $"""({flagPattern})(=|\s)?(.*)""";
    const string value_pattern = $"({quotedArgPattern})|({unquotedArgPattern})|(.*)";

    public static Arg Parse(string input)
    {
        string? value = null;
        string? sep = null;
        string? flag = null;

        var assign_regex = AssignRegex();
        var assign_match = assign_regex.Match(input);
        if (assign_match.Success)
        {
            Group g;
            g = assign_match.Groups[1];
            if (g.Value.Length > 0)
                flag = g.Value;
            g = assign_match.Groups[2];
            if (g.Value.Length > 0)
                sep = g.Value;
            g = assign_match.Groups[3];
            if (g.Value.Length > 0)
                value = g.Value;
        }
        else
        {
            value = input;
        }

        if (value is null)
            return new Arg(flag, sep, value);

        var value_regex = ValueRegex();
        var value_match = value_regex.Match(value.Trim());

        if (!value_match.Success)
            return new Arg(flag, sep, value);

        var quoted = value_match.Groups[1];
        var unquoted = value_match.Groups[2];
        var bad = value_match.Groups[3];
        if (quoted.Success)
            value = quoted.Value;
        else if (unquoted.Success)
            value = unquoted.Value;
        else if (bad.Success)
            value = bad.Value;

        var arg = new Arg(flag, sep, value);
        return arg;
    }

    [GeneratedRegex(assignPattern)]
    private static partial Regex AssignRegex();

    [GeneratedRegex(value_pattern)]
    private static partial Regex ValueRegex();
}

public readonly record struct Command
{
    public readonly string Exe;
    public readonly Arg[]? Args;
    public readonly string String;

    private Command(string exe, params Arg[]? args)
    {
        Exe = exe;
        Args = args;
        if (args is null)
        {
            String = exe;
        }
        else
        {
            String = $"{exe} {string.Join(" ", args)}";
        }
    }

    public Command Add(string s)
    {
        var arg = Arg.Parse(s);
        var cmd = Add(arg);
        return cmd;
    }

    public static Command operator +(Command a, string b)
    {
        var cmd = a.Add(b);
        return cmd;
    }

    private Command Add(Arg arg)
    {
        Command cmd;
        if (Args is null)
        {
            cmd = new Command(Exe, arg);
        }
        else
        {
            var args = new Arg[Args.Length + 1];
            Array.Copy(Args, args, Args.Length);
            args[^1] = arg;
            cmd = new Command(Exe, args);
        }

        return cmd;
    }

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
            var pargs = new Arg[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                var arg = Arg.Parse(args[i]);
                pargs[i] = arg;
            }

            cmd = new Command(exe, pargs);
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
}
