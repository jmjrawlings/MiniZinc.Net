namespace MiniZinc.Command;

using System.Runtime.CompilerServices;

/// <summary>
/// A shell command
/// </summary>
public readonly struct Command
{
    /// <summary>
    /// The executable to call, eg: "git"
    /// </summary>
    public readonly string Exe;

    /// <summary>
    /// Arguments parsed by <see cref="Args.Parse"/>
    /// </summary>
    public readonly Arg[] Arguments;

    /// <summary>
    /// Directory where the command should be run
    /// </summary>
    public readonly string? WorkingDirectory;

    public Command(string exe, Arg[]? args = null, string? workingDirectory = null)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        Arguments = args ?? Array.Empty<Arg>();
        WorkingDirectory = workingDirectory;
    }

    public Command(string exe, params string[] args)
        : this(exe, Args.Parse(args)) { }

    /// <summary>
    /// Create a new command with the given args added
    /// </summary>
    /// <example>new Command("git").Add("checkout","-b","develop")</example>
    public Command AddArgs(params string[] args) => AddArgs(Args.Parse(args));

    /// <summary>
    /// Create a new command with the given args appended
    /// </summary>
    public Command AddArgs(Arg[] args)
    {
        var arguments = Args.Concat(Arguments, args);
        var cmd = new Command(Exe, arguments);
        return cmd;
    }

    /// <summary>
    /// Returns a new command for this Exe and the given Args
    /// </summary>
    /// <example>new Command("git", "-v").WithArgs("--help") // "git --help")</example>
    public Command WithArgs(params string[] args) => AddArgs(Args.Parse(args));

    /// <summary>
    /// Returns a new command with the given working directory
    /// </summary>
    /// <example>new Command("ls").WithWorkingDirectory("/usr/local/bin")</example>
    public Command WithWorkingDirectory(string path) => new Command(Exe, Arguments, path);

    /// <summary>
    /// Returns a new command with the given working directory
    /// </summary>
    /// <example>new Command("ls").WithWorkingDirectory("/usr/local/bin")</example>
    public Command WithWorkingDirectory(DirectoryInfo dir) =>
        new Command(Exe, Arguments, dir.FullName);

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
        bool captureStdout = true,
        bool captureStderr = true,
        CancellationToken cancellation = default
    )
    {
        using var proc = new CommandRunner(this);
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
        using var proc = new CommandRunner(this);
        await foreach (var msg in proc.Watch(cancellation))
            yield return msg;
    }
}
