using System.Text.RegularExpressions;

namespace MiniZinc.Net;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Channels;
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

public readonly record struct Arg
{
    public readonly string? Flag;
    public readonly string? Value;
    public readonly string? Sep;
    public readonly string String;

    public Arg(string? flag, string? value, string? sep)
    {
        Flag = flag;
        Value = value;
        Sep = sep;
        String = $"{Flag}{Sep}{Value}";
    }

    public override string ToString() => String;

    public static Arg Parse(string input)
    {
        string? value = null;
        string? sep = null;
        string? flag = null;

        var quoted_pattern = "\"[^\"]*\"";
        var unquoted_pattern = """[^\s\"<>|&;]*""";
        var flag_pattern = """-(?:-?[\w|-]+)""";
        var assign_pattern = $"""({flag_pattern})(=|\s)?(.*)""";
        var assign_regex = new Regex(assign_pattern);
        var value_pattern = $"({quoted_pattern})|({unquoted_pattern})|(.*)";
        var value_regex = new Regex(value_pattern);
        var assign_match = assign_regex.Match(input);
        if (assign_match.Success)
        {
            flag = assign_match.Groups[1].Value;
            sep = assign_match.Groups[2].Value;
            value = assign_match.Groups[3].Value;
        }
        else
        {
            value = input;
        }

        var value_match = value_regex.Match(value.Trim());

        if (!value_match.Success)
            return new Arg(flag, sep, value);

        var quoted = value_match.Groups[1].Value;
        var unquoted = value_match.Groups[2].Value;
        var bad = value_match.Groups[3].Value;
        if (quoted.Length > 0)
            value = quoted;
        else if (unquoted.Length > 0)
            value = unquoted;
        else if (bad.Length > 0)
            value = bad;

        var arg = new Arg(flag, sep, value);
        return arg;
    }
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
            String = exe + string.Join(" ", args);
        }
    }

    public Command Add(string s)
    {
        var arg = Arg.Parse(s);
        var cmd = Add(arg);
        return cmd;
    }

    public Command Add(Arg arg)
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

    public CommandResult RunSync()
    {
        var result = Run().Result;
        return result;
    }
}
