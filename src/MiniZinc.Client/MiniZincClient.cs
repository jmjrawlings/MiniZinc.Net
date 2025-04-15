namespace MiniZinc.Client;

using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.RegularExpressions;
using Command;
using Core;

/// <summary>
/// Executes commands and solves models against
/// a given MiniZinc executable
/// </summary>
public sealed partial class MiniZincClient
{
    private readonly FileInfo _exe;
    private readonly DirectoryInfo _home;
    private readonly Version _version;
    private readonly IReadOnlyList<MiniZincSolver> _solvers;
    private readonly Dictionary<string, MiniZincSolver> _solverLookup;
    private readonly Command _command;

    public MiniZincClient(string path)
    {
        _exe = new FileInfo(path);
        if (!_exe.Exists)
            throw new FileNotFoundException(path);
        _home = _exe.Directory!;
        _command = new Command($"\"{_exe.FullName}\"");
        _version = GetVersion();
        _solvers = GetSolvers();
        _solverLookup = new Dictionary<string, MiniZincSolver>();
        foreach (var solver in _solvers)
        {
            _solverLookup[solver.Id.ToLower()] = solver;
            _solverLookup[solver.Name.ToLower()] = solver;
        }
    }

    /// <summary>
    /// The location of the MiniZinc executable
    /// </summary>
    public FileInfo Exe => _exe;

    /// <summary>
    /// The MiniZinc home directory
    /// </summary>
    public DirectoryInfo Home => _home;

    /// <summary>
    /// The version of the MiniZinc executable
    /// </summary>
    public Version Version => _version;

    public static MiniZincClient Autodetect()
    {
        var path = FindMiniZincExecutableAsync().Result;
        if (path is null)
            throw new FileNotFoundException($"Could not autodetect the MiniZinc executable");

        var client = new MiniZincClient(path);
        return client;
    }

    /// <summary>
    /// Get the installed solver corresponding to the given key
    /// where key can be:
    /// - a solver id (eg: org.minizinc.mip.highs)
    /// - a solver name (eg: coin-bc)
    /// </summary>
    public MiniZincSolver GetSolver(string key) => _solverLookup[key.ToLower()];

    /// <summary>
    /// Solve the given model, returning the best
    /// solution found or an error if it occured
    /// </summary>
    public MiniZincProcess Solve(
        MiniZincModel model,
        string? solverId = null,
        string? outputFolder = null
    )
    {
        var process = new MiniZincProcess(this, model, solverId, outputFolder);
        return process;
    }

    /// <summary>
    /// Solve the given minizinc model string, returning the best
    /// solution found or an error if it occured
    /// </summary>
    public MiniZincProcess Solve(
        string modelString,
        string? solverId = null,
        string? outputFolder = null
    )
    {
        var model = MiniZincModel.ParseString(modelString);
        var process = new MiniZincProcess(this, model, solverId, outputFolder);
        return process;
    }

    /// <summary>
    /// Get all installed solvers by running the --solvers-json
    /// command.
    /// </summary>
    private List<MiniZincSolver> GetSolvers()
    {
        var result = Command("--solvers-json").Run().Result;
        Guard.IsEqualTo(result.ExitCode, 0);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var solvers = JsonSerializer.Deserialize<List<MiniZincSolver>>(result.StdOut, options)!;
        return solvers;
    }

    private Version GetVersion()
    {
        var result = Command("--version").Run().Result;
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
    public Command Command(params string[] args)
    {
        var cmd = _command.AddArgs(args);
        return cmd;
    }

    private static async Task<string?> FindMiniZincExecutableAsync()
    {
        Command command;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            command = new Command("where", "minizinc");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            command = new Command("find", "minizinc");
        else
            throw new NotSupportedException();

        string? path = null;
        await foreach (var msg in command.Watch())
        {
            if (msg.EventType is ProcessEventType.StdOut)
            {
                path = msg.Content;
                break;
            }
        }

        return path;
    }

    [GeneratedRegex(@"MiniZinc to FlatZinc converter, version (\d).(\d).(\d), build (\d*)")]
    private static partial Regex VersionRegex();

    public override string ToString()
    {
        return $"MiniZinc {_version} (\"{_exe}\")";
    }
}
