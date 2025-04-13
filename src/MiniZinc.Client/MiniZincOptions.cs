namespace MiniZinc.Client;

using Command;

public readonly struct MiniZincOptions
{
    /// <summary>
    /// Id of the desired solver
    /// </summary>
    /// <example>gecode</example>
    /// <example>highs</example>
    public readonly string? SolverId;

    /// <summary>
    /// If given, stops the solver after the given time
    /// </summary>
    /// <remarks>This value is passed through to minizinc via the --timeout command</remarks>
    public readonly TimeSpan? Timeout;

    /// <summary>
    /// Extra arguments to pass to the command line
    /// </summary>
    public readonly Arg[]? Arguments;

    /// <summary>
    /// A folder to write outputs
    /// </summary>
    public readonly string? OutputFolder;

    public MiniZincOptions(
        string? solverId = null,
        Arg[]? args = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    )
    {
        SolverId = solverId;
        OutputFolder = outputFolder;
        Timeout = timeout;
        Arguments = args;
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
    public static MiniZincOptions Create(
        string? solverId = null,
        Arg[]? args = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    ) => new(solverId, args, outputFolder, timeout);

    public MiniZincOptions WithTimeout(TimeSpan timeout) =>
        new(SolverId, Arguments, OutputFolder, timeout);

    public MiniZincOptions WithNoTimeout() => new(SolverId, Arguments, OutputFolder, null);

    public MiniZincOptions WithOutputFolder(string path) => new(SolverId, Arguments, path, Timeout);

    public MiniZincOptions WithSolver(string solverId) =>
        new(solverId, Arguments, OutputFolder, Timeout);

    public MiniZincOptions WithSolver(Solver solver) =>
        new(solver.Id, Arguments, OutputFolder, Timeout);

    public MiniZincOptions WithArgs(params string[] args)
    {
        var opts = new MiniZincOptions(SolverId, Args.Parse(args), OutputFolder, Timeout);
        return opts;
    }

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public MiniZincOptions AddArgs(params string[] args) => AddArgs(Args.Parse(args));

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public MiniZincOptions AddArgs(params Arg[] args)
    {
        var args_ = Args.Concat(Arguments, args);
        var opts = new MiniZincOptions(SolverId, args_, OutputFolder, Timeout);
        return opts;
    }

    /// <summary>
    /// Return a new SolveOptions with the given arguments added
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public MiniZincOptions AddArgs(IEnumerable<string> args) => AddArgs(args.ToArray());
}
