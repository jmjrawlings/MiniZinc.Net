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
    /// Extra args passed to the command line
    /// </summary>
    public IEnumerable<string> ExtraArgs => _extraArgs ?? Enumerable.Empty<string>();

    /// <summary>
    /// A folder to write outputs
    /// </summary>
    public string? OutputFolder { get; }

    private readonly List<string>? _extraArgs;

    private SolveOptions(
        string? solverId = null,
        IEnumerable<string>? extraArgs = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    )
    {
        SolverId = solverId;
        OutputFolder = outputFolder;
        Timeout = timeout;
        _extraArgs = extraArgs?.ToList();
    }

    /// <summary>
    /// Create a new SolveOptions with the
    /// given arguments
    /// </summary>
    /// <param name="solverId"></param>
    /// <param name="extraArgs"></param>
    /// <param name="outputFolder"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static SolveOptions Create(
        string? solverId = null,
        IEnumerable<string>? extraArgs = null,
        string? outputFolder = null,
        TimeSpan? timeout = null
    ) => new(solverId, extraArgs, outputFolder, timeout);

    public SolveOptions WithTimeout(TimeSpan timeout) =>
        new(SolverId, _extraArgs, OutputFolder, timeout);

    public SolveOptions WithOutputFolder(string path) => new(SolverId, _extraArgs, path, Timeout);

    public SolveOptions WithSolver(string solverId) =>
        new(solverId, _extraArgs, OutputFolder, Timeout);

    public SolveOptions WithSolver(Solver solver) =>
        new(solver.Id, _extraArgs, OutputFolder, Timeout);

    public SolveOptions WithArgs(params object[] args)
    {
        var args_ = Arg.Parse(args).Select(x => x.ToString());
        var opts = new SolveOptions(SolverId, args_, OutputFolder, Timeout);
        return opts;
    }
}
