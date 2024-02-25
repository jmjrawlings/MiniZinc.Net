namespace MiniZinc.Process;

using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Diagnostics;

/// <summary>
/// Specifies a shell command and arguments (eg: git -v)
/// </summary>
public readonly record struct Command
{
    /// The program to execute eg: minizinc
    public readonly string Exe;

    /// Any arguments to provide to the command
    private readonly List<Arg>? _args;

    /// The fully qualified string eg: "git checkout -b master"
    public readonly string String;

    /// The working directory
    public readonly string? WorkingDir;

    /// <inheritdoc cref="_args"/>
    public IEnumerable<Arg> Args => _args ?? Enumerable.Empty<Arg>();

    private Command(string exe, string? workingDir = null)
    {
        Exe = exe;
        String = exe;
        WorkingDir = workingDir;
    }

    private Command(string exe, IEnumerable<Arg> args, string? workingDir = null)
    {
        Exe = exe;
        var sb = new StringBuilder(Exe);
        _args = new List<Arg>();
        foreach (var arg in args)
        {
            _args.Add(arg);
            sb.Append(' ');
            sb.Append(arg.String);
        }
        String = sb.ToString();
        WorkingDir = workingDir;
    }

    /// <summary>
    /// Return this command with the given working directory
    /// </summary>
    public Command WithWorkingDirectory(string path) => new(Exe, Args, path);

    /// <summary>
    /// Return this command with the working directory provided
    /// </summary>
    public Command WithWorkingDirectory(DirectoryInfo dir) => new(Exe, Args, dir.FullName);

    /// <summary>
    /// Create a command from the given exe and optional arguments
    /// </summary>
    /// <example>
    /// var cmd = Command.Create("git", "--version");
    /// </example>
    public static Command Create(params string[] args) => Create(Arg.Parse(args));

    /// <summary>
    /// Create a command from the given string
    /// </summary>
    /// <example>
    /// var cmd = Command.Create("git", "--version");
    /// </example>
    public static Command Create(string s) => Create(Arg.Parse(s));

    /// Create a command from the given arguments
    public static Command Create(IEnumerable<Arg> args)
    {
        var arr = args.ToArray();
        Guard.IsNotEmpty(arr);
        var exe = arr[0];
        Guard.IsTrue(exe.ArgType is ArgType.ValueOnly);
        if (arr.Length == 1)
        {
            return new Command(exe.Value!);
        }
        else
        {
            return new Command(exe.Value!, arr.Skip(1));
        }
    }

    ///
    public override string ToString()
    {
        return String;
    }

    public async Task<ProcessResult> Run(CancellationToken cancellationToken = default)
    {
        await using var proc = new Process(this, cancellationToken);
        var result = await proc.Result;
        return result;
    }
}
