namespace MiniZinc.Command;

/// <summary>
/// An immutable specification of a command line program and its arguments.
///
/// Start from <see cref="From"/> and derive variants with the <c>With…</c>
/// methods — each returns a new Command, leaving the original untouched, so a
/// base command can be safely forked:
/// <code>
/// Command mzn = Command.From("minizinc");
/// var version = mzn.WithFlag("--version").Run();
/// var solve   = mzn.WithOption("--solver", "gecode").WithValue("model.mzn").Run();
/// </code>
/// Each call to <see cref="Run"/>/<see cref="RunAsync"/>/<see cref="WatchAsync"/>
/// spawns a fresh process.
/// </summary>
public sealed class Command
{
    /// <summary>
    /// The executable to invoke, eg "minizinc".
    /// </summary>
    public string Exe { get; }

    /// <summary>
    /// The directory the command runs in, or null for the current directory.
    /// </summary>
    public string? WorkingDirectory { get; }

    /// <summary>
    /// The arguments passed to the executable.
    /// </summary>
    internal Args Arguments { get; }

    private Command(string exe, Args arguments, string? workingDirectory)
    {
        Exe = exe;
        Arguments = arguments;
        WorkingDirectory = workingDirectory;
    }

    /// <summary>
    /// Begin a command for the given executable, eg "minizinc".
    /// </summary>
    public static Command From(string exe)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentException("An executable must be provided", nameof(exe));
        return new Command(exe, new Args(), null);
    }

    // ----- Introspection -----

    /// <summary>
    /// The full argument list as it will be passed to the process (argv).
    /// </summary>
    public IEnumerable<string> Tokens => Arguments.Tokens;

    /// <summary>
    /// True if the given flag is present (bare or as "flag=value").
    /// </summary>
    public bool HasFlag(string flag) => Arguments.HasFlag(flag);

    /// <summary>
    /// Find the value supplied for an option flag ("flag value" or "flag=value").
    /// </summary>
    public bool TryGetOption(string flag, out string? value) =>
        Arguments.TryGetOption(flag, out value);

    // ----- Derivation (each returns a new Command) -----

    /// <summary>
    /// A copy with one or more literal argument tokens appended
    /// (a leading '-' marks a flag).
    /// </summary>
    public Command With(params string?[] tokens) => Derive(args => args.Add(tokens));

    /// <summary>
    /// A copy with a bare flag appended, eg "--all-solutions".
    /// </summary>
    public Command WithFlag(string flag) => Derive(args => args.AddFlag(flag));

    /// <summary>
    /// A copy with a flag/value pair appended, eg ("--solver", "gecode").
    /// </summary>
    public Command WithOption(string flag, string value, bool eq = false) =>
        Derive(args => args.AddOption(flag, value, eq));

    /// <summary>
    /// A copy with a positional value appended, eg a model file path.
    /// </summary>
    public Command WithValue(string value) => Derive(args => args.AddValue(value));

    /// <summary>
    /// A copy with a raw, user-supplied argument string tokenised and appended.
    /// </summary>
    public Command WithCommandLine(string commandLine) =>
        Derive(args => args.AddCommandLine(commandLine));

    /// <summary>
    /// A copy that runs in the given working directory.
    /// </summary>
    public Command WithWorkingDirectory(string workingDirectory) =>
        new Command(Exe, Arguments.Copy(), workingDirectory);

    private Command Derive(Action<Args> append)
    {
        Args next = Arguments.Copy();
        append(next);
        return new Command(Exe, next, WorkingDirectory);
    }

    // ----- Execution (each spawns a fresh process) -----

    /// <summary>
    /// Run to completion (or until cancelled), capturing stdout and stderr.
    /// </summary>
    public Task<ProcessResult> RunAsync(CancellationToken cancellation = default) =>
        new CommandProcess(this).Run(cancellation);

    /// <summary>
    /// Synchronously run to completion. Blocks the calling thread.
    /// </summary>
    public ProcessResult Run(CancellationToken cancellation = default) =>
        Task.Run(() => RunAsync(cancellation)).GetAwaiter().GetResult();

    /// <summary>
    /// Run and stream messages as they happen.
    /// </summary>
    public IAsyncEnumerable<ProcessMessage> WatchAsync(CancellationToken cancellation = default) =>
        new CommandProcess(this).Watch(cancellation);

    public override string ToString() =>
        Arguments.Count == 0 ? Exe : $"{Exe} {Arguments}";
}
