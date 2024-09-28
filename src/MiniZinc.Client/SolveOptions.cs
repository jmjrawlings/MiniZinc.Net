namespace MiniZinc.Client;

using Command;

public readonly struct SolveOptions
{
    /// <summary>
    /// Id of the desired solver
    /// </summary>
    /// <example>gecode</example>
    /// <example>highs</example>
    public string? SolverId { get; }

    /// <summary>
    /// If given, stops the solver after the given time
    /// </summary>
    /// <remarks>This value is passed through to minizinc via the --timeout command</remarks>
    public TimeSpan? Timeout { get; }

    /// <summary>
    /// Extra arguments to pass to the command line
    /// </summary>
    public Arg[] Arguments { get; }

    /// <summary>
    /// A folder to write outputs
    /// </summary>
    public string? OutputFolder { get; }

    public SolveOptions(
        string? solverId = null,
        Arg[]? args = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    )
    {
        SolverId = solverId;
        OutputFolder = outputFolder;
        Timeout = timeout;
        Arguments = args ?? [];
    }

    /// <summary>
    /// Create a new SolveOptions with the
    /// given arguments
    /// </summary>
    /// <param name="solverId"></param>
    /// <param name="args"></param>
    /// <param name="outputFolder"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static SolveOptions Create(
        string? solverId = null,
        Arg[]? args = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    ) => new(solverId, args, outputFolder, timeout);

    public SolveOptions WithTimeout(TimeSpan timeout) =>
        new(SolverId, Arguments, OutputFolder, timeout);

    public SolveOptions WithNoTimeout() => new(SolverId, Arguments, OutputFolder, null);

    public SolveOptions WithOutputFolder(string path) => new(SolverId, Arguments, path, Timeout);

    public SolveOptions WithSolver(string solverId) =>
        new(solverId, Arguments, OutputFolder, Timeout);

    public SolveOptions WithSolver(Solver solver) =>
        new(solver.Id, Arguments, OutputFolder, Timeout);

    public SolveOptions WithArgs(params string[] args)
    {
        var opts = new SolveOptions(SolverId, Args.Parse(args), OutputFolder, Timeout);
        return opts;
    }

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public SolveOptions AddArgs(params string[] args) => AddArgs(Args.Parse(args));

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public SolveOptions AddArgs(params Arg[] args)
    {
        var args_ = Args.Concat(Arguments, args);
        var opts = new SolveOptions(SolverId, args_, OutputFolder, Timeout);
        return opts;
    }

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public SolveOptions AddArgs(IEnumerable<string> args) => AddArgs(args.ToArray());
}
