namespace MiniZinc.Client;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Command;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Parser;
using Parser.Syntax;

/// <summary>
/// Executes commands and solves models against
/// a given MiniZinc executable
/// </summary>
public sealed partial class MiniZincClient
{
    private readonly FileInfo _exe;
    private readonly DirectoryInfo _home;
    private readonly Version _version;
    private readonly IReadOnlyList<Solver> _solvers;
    private readonly Dictionary<string, Solver> _solverLookup;
    private readonly Command _command;
    private readonly ILogger _logger;

    private MiniZincClient(FileInfo exe, ILogger? logger = null)
    {
        _exe = exe;
        _home = exe.Directory!;
        _command = new Command($"\"{_exe.FullName}\"");
        _logger = logger ?? NullLogger.Instance;
        _version = GetVersion();
        _solvers = GetSolvers();
        _solverLookup = new Dictionary<string, Solver>();
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

    /// <summary>
    /// Get the installed solver corresponding to the given key
    /// where key can be:
    /// - a solver id (eg: org.minizinc.mip.highs)
    /// - a solver name (eg: coin-bc)
    /// </summary>
    public Solver GetSolver(string key) => _solverLookup[key.ToLower()];

    /// <summary>
    /// Solve the given model, returning the best
    /// solution found or an error if it occured
    /// </summary>
    public async Task<Solution> Solve(
        Model model,
        SolveOptions? options = default,
        CancellationToken token = default
    )
    {
        Solution? best = null;
        await foreach (var solution in Solutions(model, options, token))
        {
            best = solution;
        }

        return best!;
    }

    /// <summary>
    /// Solve the given model with the given options
    /// </summary>
    public async IAsyncEnumerable<Solution> Solutions(
        Model model,
        SolveOptions? options = default,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        model.EnsureOk();
        var modelText = model.SourceText;
        var solver = GetSolver(options?.SolverId ?? Solver.Gecode);
        var method = model.Method;
        var outputPath = options?.OutputFolder ?? Path.GetTempPath();
        var modelPath = Path.Combine(
            outputPath,
            Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + ".mzn"
        );
        await File.WriteAllTextAsync(modelPath, modelText, token);
        _logger.LogInformation("Model saved to {Path}", modelPath);

        var command = Command(
            "--solver",
            solver.Id,
            "--json-stream",
            "--output-objective",
            modelPath
        );
        var solveStart = DateTimeOffset.Now;
        var iterStart = solveStart;
        var iteration = 0;
        var data = new Dictionary<string, SyntaxNode>();
        var warnings = new List<string>();
        var status = SolveStatus.Started;
        var text = "";
        int objective = 0;
        int processId = 0;
        await foreach (var msg in command.Watch().WithCancellation(token))
        {
            JsonOutput? message = null;
            switch (msg.EventType, msg.Content)
            {
                case (ProcessEventType.Started, _):
                    status = SolveStatus.Started;
                    processId = msg.ProcessId;
                    break;
                case (ProcessEventType.StdErr, var s):
                    message = JsonOutput.Deserialize(s!);
                    break;
                case (ProcessEventType.StdOut, var s):
                    message = JsonOutput.Deserialize(s!);
                    break;
                case (ProcessEventType.Exited, _):
                    break;
            }

            if (message is null)
                continue;

            switch (message)
            {
                case StatusOutput m:
                    _logger.LogInformation("Status {Status}", m.Status);

                    switch (m.Status)
                    {
                        case "ALL_SOLUTIONS":
                            status = SolveStatus.AllSolutions;
                            break;
                        case "OPTIMAL_SOLUTION":
                            status = SolveStatus.Optimal;
                            break;
                        case "UNSATISFIABLE":
                            status = SolveStatus.Unsatisfiable;
                            break;
                        case "UNBOUNDED":
                            status = SolveStatus.Unbounded;
                            break;
                        case "UNSAT_OR_UNBOUNDED":
                            status = SolveStatus.UnsatOrUnbounded;
                            break;
                        case "UNKNOWN":
                            status = SolveStatus.Error;
                            break;
                        case "ERROR":
                            status = SolveStatus.Error;
                            break;
                    }
                    break;

                case SolutionOutput m:
                    iteration++;
                    status = SolveStatus.Satisfied;
                    text = m.Output["dzn"].ToString()!;
                    var parsed = Parser.ParseString(text);
                    foreach (var node in parsed.SyntaxNode.Nodes)
                    {
                        if (node is not AssignmentSyntax assign)
                            throw new Exception();
                        var name = assign.Name.ToString();
                        var value = assign.Expr;
                        data[name] = value;
                    }

                    if (data.TryGetValue("_objective", out var obj))
                    {
                        objective = (IntLiteralSyntax)obj;
                    }
                    break;
                case CommentOutput m:
                    _logger.LogInformation("{Comment}", m.Comment);
                    break;
                case WarningOutput m:
                    _logger.LogWarning("{Kind} - {Message}", m.Kind, m.Message);
                    break;
                case ErrorOutput m:
                    _logger.LogError("{Kind} - {Message}", m.Kind, m.Message);
                    break;
                case StatOutput m:
                    break;
                case TraceOutput m:
                    break;
            }

            var time = DateTimeOffset.Now;
            var solveTime = solveStart.ToPeriod(time);
            var iterTime = iterStart.ToPeriod(time);
            var solution = new Solution
            {
                Command = command,
                ProcessId = processId,
                TotalTime = solveTime,
                IterationTime = iterTime,
                Iteration = iteration,
                Status = status,
                Objective = objective,
                Bound = null,
                AbsoluteGap = null,
                RelativeGap = null,
                AbsoluteDelta = null,
                RelativeDelta = null,
                Text = text,
                Data = data,
                Outputs = null,
                Statistics = null,
                Warnings = null
            };
            iterStart = time;
            yield return solution;
        }
    }

    /// <summary>
    /// Get all installed solvers by running the --solvers-json
    /// command.
    /// </summary>
    public IReadOnlyList<Solver> GetSolvers()
    {
        var result = Command("--solvers-json").Run().Result;
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
    public Command Command(params object[] args)
    {
        var cmd = _command.Add(args);
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

    /// <summary>
    /// Find the location of the minizinc executable
    /// on the current system.
    /// </summary>
    /// <returns>Path to the `minizinc` executable if found</returns>
    public static string? FindMiniZincExecutable()
    {
        var exe = FindMiniZincExecutableAsync().Result;
        return exe;
    }

    /// <summary>
    /// Create a client for the given minizinc executable.
    /// If no path is provided, attempt to search for installed exe
    /// </summary>
    /// <param name="exe">Filepath of the minizinc executable</param>
    public static MiniZincClient Create(string? exe = null)
    {
        exe ??= FindMiniZincExecutable();
        Guard.IsNotNull(exe);
        var client = Create(new FileInfo(exe));
        return client;
    }

    /// <summary>
    /// Create a client for the given MiniZinc executable
    /// </summary>
    /// <param name="exe">The minizinc executable</param>
    public static MiniZincClient Create(FileInfo exe)
    {
        Guard.IsTrue(exe.Exists);
        var client = new MiniZincClient(exe);
        return client;
    }

    public override string ToString()
    {
        return $"MiniZinc Client (\"{_exe}\")";
    }

    [GeneratedRegex(@"MiniZinc to FlatZinc converter, version (\d).(\d).(\d), build (\d*)")]
    private static partial Regex VersionRegex();

    /// <summary>
    /// MiniZinc messages from --json-output
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(SolutionOutput), typeDiscriminator: "solution")]
    [JsonDerivedType(typeof(StatOutput), typeDiscriminator: "statistics")]
    [JsonDerivedType(typeof(CommentOutput), typeDiscriminator: "comment")]
    [JsonDerivedType(typeof(TraceOutput), typeDiscriminator: "trace")]
    [JsonDerivedType(typeof(ErrorOutput), typeDiscriminator: "error")]
    [JsonDerivedType(typeof(WarningOutput), typeDiscriminator: "warning")]
    [JsonDerivedType(typeof(StatusOutput), typeDiscriminator: "status")]
    private record JsonOutput
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions;

        static JsonOutput()
        {
            JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public static JsonOutput Deserialize(string json)
        {
            var message = JsonSerializer.Deserialize<JsonOutput>(json, JsonSerializerOptions);
            return message!;
        }
    }

    private sealed record WarningOutput : ErrorOutput { }

    private sealed record StatOutput : JsonOutput
    {
        public required JsonObject Statistics { get; init; }
    }

    private sealed record TraceOutput : JsonOutput
    {
        public string Section { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;
    }

    private sealed record MiniZincErrorLocationMessage
    {
        public string Filename { get; init; } = string.Empty;
        public int FirstLine { get; init; }
        public int FirstColumn { get; init; }
        public int LastLine { get; init; }
        public int LastColumn { get; init; }
    }

    private sealed record CommentOutput : JsonOutput
    {
        [JsonPropertyName("comment")]
        public required string Comment { get; init; }
    }

    private record ErrorOutput : JsonOutput
    {
        [JsonPropertyName("what")]
        public string Kind { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;

        public MiniZincErrorLocationMessage? Location { get; init; }

        public IEnumerable<MiniZincErrorStack>? Stack { get; init; }
    }

    private sealed record MiniZincErrorStack
    {
        public required MiniZincErrorLocationMessage Location { get; init; }

        public bool IsCompIter { get; init; }

        public required string Description { get; init; }
    }

    /// <summary>
    /// The solution message provided by the
    /// the solver.
    /// "https://www.minizinc.org/doc-latest/en/json-stream.html"
    /// </summary>
    private sealed record SolutionOutput : JsonOutput
    {
        public int? Time { get; set; }

        public List<string>? Sections { get; set; }

        public Dictionary<string, object>? Output { get; set; }
    }

    private sealed record StatusOutput : JsonOutput
    {
        public required string Status { get; init; }

        public int? Time { get; init; }
    }
}
