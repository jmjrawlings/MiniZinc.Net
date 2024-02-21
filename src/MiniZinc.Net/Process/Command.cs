namespace MiniZinc.Net.Process;

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;

/// <summary>
/// Specifies a shell command and arguments (eg: git -v)
/// </summary>
public readonly record struct Command
{
    /// The program to execute eg: minizinc
    public readonly string Exe;

    /// The fully qualified string eg: "git checkout -b master"
    public readonly string String;

    /// The working directory
    public readonly string? WorkingDir;

    private readonly List<Arg> _args;
    public IReadOnlyList<Arg> Arguments => _args;

    private Command(string exe, IEnumerable<Arg>? args = null, string? workingDir = null)
    {
        Exe = exe;
        WorkingDir = workingDir;
        _args = new List<Arg>();
        if (args is not null)
            _args.AddRange(args);

        if (_args.Count == 0)
        {
            String = exe;
        }
        else
        {
            var sb = new StringBuilder();
            sb.Append(Exe);
            sb.Append(' ');
            sb.AppendJoin(' ', _args);
            String = sb.ToString();
        }
    }

    /// <summary>
    /// Return this command with the given working directory
    /// </summary>
    public Command WithWorkingDirectory(string path) => new(Exe, _args, path);

    /// <summary>
    /// Return this command with the working directory provided
    /// </summary>
    public Command WithWorkingDirectory(DirectoryInfo dir) => new(Exe, _args, dir.FullName);

    /// <summary>
    /// Return this command with the given arguments
    /// </summary>
    public Command WithArgs(string[] args) => new(Exe, Args.Parse(args), WorkingDir);

    /// <summary>
    /// Create a command from the given exe and optional arguments
    /// </summary>
    /// <example>
    /// var cmd = Command.Create("git", "--version");
    /// </example>
    public static Command Create(string exe, params string[] args)
    {
        Guard.IsNotNullOrWhiteSpace(exe);
        var cmd = new Command(exe, Args.Parse(args));
        return cmd;
    }

    /// Parse a command from the given string
    public static Command Parse(string s)
    {
        Guard.IsNotNullOrWhiteSpace(s);
        var args = Args.Parse(s).ToArray();
        var exe = args[0].ToString();
        var cmd = new Command(exe, args.Skip(1));
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
