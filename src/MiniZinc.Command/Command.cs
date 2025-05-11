namespace MiniZinc.Command;

using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// A shell command
/// </summary>
public struct Command
{
    /// <summary>
    /// The executable to call, eg: "git"
    /// </summary>
    public readonly string Exe;

    /// <summary>
    /// Arguments parsed by <see cref="Args.Parse"/>
    /// </summary>
    public readonly Args Arguments;

    /// <summary>
    /// Directory where the command should be run
    /// </summary>
    public string? WorkingDirectory;

    public Command(string exe, params string[] args)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        Arguments = Args.Parse(args);
    }

    public Command(string exe, Args args)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        Arguments = args;
    }

    public Command(string exe)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        Arguments = Args.Empty;
    }

    /// <summary>
    /// Create a new command with the given args added
    /// </summary>
    /// <example>new Command("git").Add("checkout","-b","develop")</example>
    public void AddArgs(params string?[] args)
    {
        Arguments.Add(args);
    }

    /// <summary>
    /// Create a new command with the given args appended
    /// </summary>
    public void AddArgs(Args args)
    {
        Arguments.Add(args);
    }

    public async Task<ProcessResult> Run(
        bool captureStdout = true,
        bool captureStderr = true,
        CancellationToken cancellation = default
    )
    {
        var proc = new CommandProcess(this);
        var result = await proc.Run(
            captureStdErr: captureStderr,
            captureStdOut: captureStdout,
            cancellation: cancellation
        );
        return result;
    }

    public async IAsyncEnumerable<ProcessMessage> Watch(
        [EnumeratorCancellation] CancellationToken cancellation = default
    )
    {
        var proc = new CommandProcess(this);
        await foreach (var msg in proc.Watch(cancellation))
            yield return msg;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Exe);
        foreach (var arg in Arguments.Values)
        {
            sb.Append(' ');
            sb.Append(arg);
        }

        var cmd = sb.ToString();
        return cmd;
    }
}
