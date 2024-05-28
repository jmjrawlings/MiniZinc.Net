namespace MiniZinc.Client;

using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Parser;
using Parser.Syntax;
using Process;

/// <summary>
/// MiniZinc Client
/// </summary>
public sealed partial class MiniZincClient
{
    public readonly FileInfo Executable;
    private readonly ILogger _logger;
    private readonly Version _version;
    private readonly List<Solver> _solvers;
    private readonly Dictionary<string, Solver> _solverLookup;

    /// <summary>
    /// The version of the minizinc executable
    /// </summary>
    public Version Version => _version;

    /// <summary>
    /// All installed solvers discovered through `--solvers-json`
    /// </summary>
    public IEnumerable<Solver> Solvers => _solvers;

    public Solver GetSolver(string key) => _solverLookup[key];

    public Instance SolveModelFile(string modelPath, SolveOptions? options = null)
    {
        var parsed = Parser.ParseFile(modelPath);
        Guard.IsTrue(parsed.Ok);
        var model = parsed.Syntax;
        var process = SolveModel(model, options);
        return process;
    }
    
    public Instance SolveModelText(string modelText, SolveOptions? options = null)
    {
        var parsed = Parser.ParseText(modelText);
        Guard.IsTrue(parsed.Ok);
        var model = parsed.Syntax;
        var process = SolveModel(model, options);
        return process;
    }

    public Instance SolveModel(SyntaxTree model, SolveOptions? options = null)
    {
        var process = new Instance(this, model, options ?? new SolveOptions());
        return process;
    }

    private MiniZincClient(FileInfo executable, ILogger? logger = null)
    {
        Executable = executable;
        _logger = logger ?? NullLogger.Instance;
        _version = GetVersion();
        _solvers = GetInstalledSolvers();
        _solverLookup = new Dictionary<string, Solver>();
        foreach (var solver in _solvers)
        {
            var code = solver.Id.Split('.')[^1];
            _solverLookup[solver.Id] = solver;
            _solverLookup[solver.Name] = solver;
            _solverLookup[code] = solver;
        }
    }

    private List<Solver> GetInstalledSolvers()
    {
        var result = CreateProcess("--solvers-json").WaitSync();
        Guard.IsEqualTo(result.ExitCode, 0);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var solvers = JsonSerializer.Deserialize<List<Solver>>(result.StdOut, options)!;
        foreach (var solver in solvers)
        {
            _logger.LogInformation(
                "Found solver {Id}: {Solver} {Version}",
                solver.Id,
                solver.Name,
                solver.Version
            );
        }

        return solvers;
    }

    private Version GetVersion()
    {
        var result = CreateProcess("--version").WaitSync();
        Guard.IsEqualTo(result.ExitCode, 0);
        var match = VersionRegex().Match(result.StdOut);
        var version = new Version(
            int.Parse(match.Groups[1].Value),
            int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value),
            int.Parse(match.Groups[4].Value)
        );
        return version;
    }

    /// <summary>
    /// Create a minizinc command with the given arguments
    /// </summary>
    public Command CreateCommand(IEnumerable<object> args)
    {
        var sb = new StringBuilder();
        sb.Append('"');
        sb.Append(Executable.FullName);
        sb.Append('"');
        sb.Append(' ');
        sb.AppendJoin(' ', args);
        var command = Command.Create(sb.ToString());
        return command;
    }

    /// <summary>
    /// Create a minizinc process for the given command line arguments
    /// </summary>
    public Process CreateProcess(params object[] args)
    {
        var command = CreateCommand(args);
        var process = new Process(command, _logger);
        return process;
    }

    /// <summary>
    /// Create a new MiniZinc client
    /// </summary>
    /// <param name="path">Filepath of the minizinc executable</param>
    /// <returns></returns>
    public static MiniZincClient Create(string path)
    {
        var exe = new FileInfo(path);
        var client = Create(exe);
        return client;
    }

    /// <summary>
    /// Create a new MiniZinc client
    /// </summary>
    /// <param name="exe">The minizinc executable</param>
    public static MiniZincClient Create(FileInfo exe)
    {
        var client = new MiniZincClient(exe);
        return client;
    }

    public override string ToString()
    {
        return $"MiniZinc Client \"{Executable}";
    }

    [GeneratedRegex(@"MiniZinc to FlatZinc converter, version (\d).(\d).(\d), build (\d*)")]
    private static partial Regex VersionRegex();
}
