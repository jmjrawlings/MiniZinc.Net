namespace MiniZinc.Net;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

/// <summary>
/// Specifies a shell command and arguments (eg: git -v)
/// </summary>
public readonly record struct Command
{
    /// <summary>
    /// The program to execute eg: minizinc
    /// </summary>
    public readonly string Exe;

    /// <summary>
    /// The arguments provided to the exe
    /// </summary>
    public IEnumerable<string> Args =>
        _args?.Select(a => a.ToString()) ?? Enumerable.Empty<string>();

    /// <summary>
    /// The fully qualified string eg: "git checkout -b master"
    /// </summary>
    public readonly string String;

    /// <summary>
    /// The working directory
    /// </summary>
    public readonly string? WorkingDir;

    private readonly CommandArg[]? _args;

    private Command(string exe, CommandArg[]? args = null, string? workingDir = null)
    {
        Exe = exe;
        WorkingDir = workingDir;
        _args = args;

        if (args is null || args.Length == 0)
        {
            String = exe;
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append(Exe);
            sb.Append(' ');
            sb.AppendJoin(' ', args);
            String = sb.ToString();
        }
    }

    /// <summary>
    /// Return this command with the given arguments
    /// </summary>
    public Command WithArgs(params string[]? args) =>
        Create(Exe, args).WithWorkingDirectory(WorkingDir);

    /// <summary>
    /// Return this command with the given working directory
    /// </summary>
    public Command WithWorkingDirectory(string path) => new(Exe, _args, path);

    /// <summary>
    /// Return this command with the working directory provided
    /// </summary>
    public Command WithWorkingDirectory(DirectoryInfo dir) => new(Exe, _args, dir.FullName);

    /// <summary>
    /// Create a command from the given exe and optional arguments
    /// </summary>
    /// <example>
    /// var cmd = Command.Create("git", "--version");
    /// </example>
    public static Command Create(string exe, params string[] args)
    {
        Guard.IsNotNullOrWhiteSpace(exe);
        var pargs = CommandArgs.Parse(args).ToArray();
        var cmd = new Command(exe, pargs);
        return cmd;
    }

    public static Command Create(string command)
    {
        Guard.IsNotNullOrWhiteSpace(command);
        var pargs = CommandArgs.Parse(command).ToArray();
        var exe = pargs[0].ToString();
        var cargs = new CommandArg[pargs.Length - 1];
        Array.Copy(pargs, cargs, cargs.Length);
        var cmd = new Command(exe, cargs);
        return cmd;
    }

    /// <summary>
    /// Create a process for this command
    /// </summary>
    public Process ToProcess(
        bool stdout = true,
        bool stderr = true,
        CancellationToken cancellationToken = default
    )
    {
        var process = new Process(this, stdout, stderr, cancellationToken);
        return process;
    }

    public async Task<ProcessResult> Run(
        bool stdout = true,
        bool stderr = true,
        CancellationToken cancellationToken = default
    )
    {
        using var process = new Process(this, stdout, stderr, cancellationToken);
        var result = await process.Result;
        return result;
    }

    public override string ToString()
    {
        return String;
    }
}
