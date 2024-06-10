namespace MiniZinc.Command;

using System.Runtime.CompilerServices;

/// <summary>
/// A shell command
/// </summary>
public readonly struct Command
{
    public readonly string Exe;

    public readonly Arg[] Arguments;

    public Command(string exe, Arg[]? args = null)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        Arguments = args ?? Array.Empty<Arg>();
    }

    public Command(string exe, IEnumerable<Arg> args)
        : this(exe, args.ToArray()) { }

    public Command(string exe, params string[] args)
        : this(exe, Args.Parse(args)) { }

    /// <summary>
    /// Create a new command with the given args added
    /// </summary>
    /// <example>new Command("git").AddArgs("checkout","-b","develop")</example>
    public Command AddArgs(params string[] args) => AddArgs(Args.Parse(args));

    /// <summary>
    /// Create a new command with the given args added
    /// </summary>
    public Command AddArgs(Arg[] args)
    {
        var args_ = Args.Concat(Arguments, args);
        var cmd = new Command(Exe, args_);
        return cmd;
    }

    public override string ToString()
    {
        string s;
        if (Arguments.Length is 0)
            s = Exe;
        else
            s = $"{Exe} {string.Join(' ', Arguments)}";
        return s;
    }

    public async Task<ProcessResult> Run(
        string? workingDir = null,
        bool captureStdout = true,
        bool captureStderr = true,
        CancellationToken cancellation = default
    )
    {
        using var proc = new CommandRunner(this, workingDir);
        var result = await proc.Run(
            captureStdErr: captureStderr,
            captureStdOut: captureStdout,
            cancellation: cancellation
        );
        return result;
    }

    public async IAsyncEnumerable<ProcessMessage> Watch(
        string? workingDir = null,
        [EnumeratorCancellation] CancellationToken cancellation = default
    )
    {
        using var proc = new CommandRunner(this, workingDir);
        await foreach (var msg in proc.Watch(cancellation))
            yield return msg;
    }
}
