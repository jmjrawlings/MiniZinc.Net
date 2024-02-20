using System.Runtime.CompilerServices;

namespace MiniZinc.Net;

using System.Collections.Generic;
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
    public readonly DirectoryInfo? WorkingDir;

    private readonly CommandArg[]? _args;

    private Command(string exe, CommandArg[]? args = null, DirectoryInfo? workingDir = null)
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
            foreach (var arg in args)
            {
                var s = arg.ToString();
                sb.Append(' ');
                sb.Append(s);
            }

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
    public Command WithWorkingDirectory(string path) =>
        WithWorkingDirectory(new DirectoryInfo(path));

    /// <summary>
    /// Return this command with the working directory provided
    /// </summary>
    public Command WithWorkingDirectory(DirectoryInfo? dir) => new(Exe, _args, dir);

    /// <summary>
    /// Create a command from the given exe and optional arguments
    /// </summary>
    /// <example>
    /// var cmd = Command.Create("git", "--version");
    /// </example>
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
            var arr = CommandArgs.Parse(args).ToArray();
            cmd = new Command(exe, arr);
        }

        return cmd;
    }

    /// <summary>
    /// Run the command until termination
    /// </summary>
    public async Task<CommandResult> Run(bool stdout = true, bool stderr = true)
    {
        var runner = new CommandRunner(this, stdout, stderr);
        var result = await runner.Run();
        return result;
    }

    /// <summary>
    /// Run the command and stream messages
    /// </summary>
    public async IAsyncEnumerable<CommandMessage> Listen(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var listener = new CommandListener(this);
        await foreach (var msg in listener.Listen(cancellationToken))
        {
            yield return msg;
        }
    }
}
