namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Command;
using Core;
using Parser;

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

    public MiniZincClient(string path)
    {
        _exe = new FileInfo(path);
        if (!_exe.Exists)
            throw new FileNotFoundException(path);
        _home = _exe.Directory!;
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
        var path = FindMiniZincExecutable();
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
    /// Get all installed solvers by running the --solvers-json
    /// command.
    /// </summary>
    private List<MiniZincSolver> GetSolvers()
    {
        var result = Cmd("--solvers-json").Run();
        Guard.IsEqualTo(result.ExitCode, 0);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var solvers = JsonSerializer.Deserialize<List<MiniZincSolver>>(result.StdOut, options)!;
        return solvers;
    }

    private Version GetVersion()
    {
        var result = Cmd("--version").Run();
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
    /// Create a command for the minizinc executable with the given arguments.
    /// </summary>
    private Command Cmd(params string[] args)
    {
        // No surrounding quotes: with UseShellExecute=false the exe path is used
        // verbatim as the process filename, so quotes would become part of the
        // path and the launch fails (esp. on Linux).
        return Command.From(_exe.FullName).With(args);
    }

    private static string? FindMiniZincExecutable()
    {
        // `where` on Windows, `which` everywhere else (Linux + macOS).
        var finder = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "where" : "which";
        var result = Command.From(finder).With("minizinc").Run();
        if (result.ExitCode != 0)
            return null;

        // `where` can return several matches; take the first.
        var path = result
            .StdOut.Split(
                '\n',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            )
            .FirstOrDefault();
        return string.IsNullOrWhiteSpace(path) ? null : path;
    }

    /// <summary>
    /// Legacy overload: solve with an optional solver and raw command-line args.
    /// Delegates to the <see cref="SolveOptions"/> core.
    /// </summary>
    public Task<MiniZincMessage> Solution(
        MiniZincModel model,
        string? solver = null,
        CancellationToken token = default,
        params string?[] args
    ) => Solution(model, new SolveOptions { Solver = solver, ExtraArgs = ToExtraArgs(args) }, token);

    /// <summary>
    /// Run the model to completion and return the final <see cref="MiniZincMessage"/>.
    /// </summary>
    public async Task<MiniZincMessage> Solution(
        MiniZincModel model,
        SolveOptions options,
        CancellationToken token = default
    )
    {
        MiniZincMessage? msg = null;
        await foreach (var message in Solve(model, options, token))
        {
            msg = message;
        }

        if (msg is null)
            throw new Exception($"No message returned");

        return msg;
    }

    /// <summary>
    /// Legacy overload: solve with an optional solver and raw command-line args.
    /// Delegates to the <see cref="SolveOptions"/> core.
    /// </summary>
    public IAsyncEnumerable<MiniZincMessage> Solve(
        MiniZincModel model,
        string? solver = null,
        CancellationToken token = default,
        params string?[] args
    ) => Solve(model, new SolveOptions { Solver = solver, ExtraArgs = ToExtraArgs(args) }, token);

    private static IReadOnlyList<string>? ToExtraArgs(string?[] args)
    {
        if (args.Length == 0)
            return null;
        List<string>? list = null;
        foreach (string? arg in args)
            if (arg is not null)
                (list ??= new List<string>()).Add(arg);
        return list;
    }

    public async IAsyncEnumerable<MiniZincMessage> Solve(
        MiniZincModel model,
        SolveOptions options,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        // `(model, null, token)` resolves here (the params-less overload wins over
        // the legacy params overload); tolerate it by falling back to defaults.
        options ??= new SolveOptions();

        if (token.IsCancellationRequested)
        {
            yield return new MiniZincMessage
            {
                Command = "",
                ProcessId = 0,
                Solver = null,
                TimeStamp = DateTimeOffset.Now,
                Status = SolveStatus.Cancelled
            };
            yield break;
        }

        // A timeout becomes a token cancellation linked to the caller's token, so
        // the existing tree-kill on cancel tears down the whole solver process
        // group rather than orphaning it.
        CancellationTokenSource? timeoutCts = null;
        if (options.Timeout is { } timeout)
        {
            timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            timeoutCts.CancelAfter(timeout);
        }
        CancellationToken cancellation = timeoutCts?.Token ?? token;

        var directory = Path.GetTempPath();
        string modelString = model.Write();
        string modelFile = Path.Join(
            directory,
            $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.mzn"
        );

        await File.WriteAllTextAsync(modelFile, modelString, cancellation);
        try
        {
            Command command = Cmd();
            if (options.ExtraArgs is { } extraArgs)
                foreach (string arg in extraArgs)
                    command = command.WithCommandLine(arg);

            string? solver = options.Solver;
            if (command.TryGetOption("--solver", out string? solverArg))
            {
                if (solver is not null)
                    throw new ArgumentException(
                        $"Solver was provided both as an argument and command line"
                    );
                solver = solverArg;
            }

            if (options.CompileOnly)
            {
                command = command.WithFlag("--compile");
                if (options.OutputFile is { } outputFile)
                    command = command.WithOption("--output-to-file", outputFile);
            }
            else
            {
                command = command
                    .WithFlag("--json-stream")
                    .WithFlag("--output-objective")
                    .WithFlag("--statistics");
            }

            solver ??= MiniZincSolver.GECODE;
            var solverInfo = GetSolver(solver);
            command = command.WithOption("--solver", solverInfo.Id);

            if (options.SearchPaths is { } searchPaths)
                foreach (string searchPath in searchPaths)
                    command = command.WithOption("-I", searchPath);

            if (options.ExtraModelFiles is { } extraModelFiles)
                foreach (string file in extraModelFiles)
                    command = command.WithValue(file);

            if (options.DataFiles is { } dataFiles)
                foreach (string file in dataFiles)
                    command = command.WithValue(file);

            command = command.WithValue(modelFile);
            var commandString = command.ToString();

            Dictionary<string, JsonValue>? statistics = null;
            List<string>? warnings = null;
            int iteration = 0;
            DateTimeOffset startTime = DateTimeOffset.Now;
            DateTimeOffset lastTime = startTime;
            DateTimeOffset endTime = startTime;
            TimeSpan iterTime = TimeSpan.Zero;
            TimeSpan totalTime = TimeSpan.Zero;
            StringBuilder? stderr = null;

            MiniZincMessage msg = new MiniZincMessage
            {
                Command = commandString,
                Model = modelString,
                Solver = solverInfo,
                TimeStamp = startTime
            };

            // The command runner owns the process, multiplexes stdout/stderr onto
            // a single stream (no two-pipe deadlock), and kills the process on
            // cancellation — surfacing the kill as a normal ProcessExited rather
            // than an exception, so cancellation is detected via the token below.
            await foreach (ProcessMessage message in command.WatchAsync(cancellation))
            {
                switch (message)
                {
                    case ProcessStarted started:
                        msg = msg with { ProcessId = started.ProcessId };
                        break;

                    case ProcessStdOut stdout:
                        endTime = DateTimeOffset.Now;
                        iterTime = endTime - lastTime;
                        totalTime = endTime - startTime;
                        lastTime = endTime;
                        bool emit = false;
                        // Compile-only output is FlatZinc text, not a json-stream;
                        // full capture/compare lands in a later phase.
                        if (options.CompileOnly)
                            break;
                        JsonOutput output = JsonOutput.Deserialize(stdout.Text);
                        switch (output)
                        {
                            case StatusOutput o:
                                msg = msg with
                                {
                                    TimeStamp = endTime,
                                    TotalTime = totalTime,
                                    Status = o.Status switch
                                    {
                                        "ALL_SOLUTIONS" => SolveStatus.AllSolutions,
                                        "OPTIMAL_SOLUTION" => SolveStatus.Optimal,
                                        "UNSATISFIABLE" => SolveStatus.Unsatisfiable,
                                        "UNBOUNDED" => SolveStatus.Unbounded,
                                        "UNSAT_OR_UNBOUNDED" => SolveStatus.UnsatOrUnbounded,
                                        "ERROR" => SolveStatus.Error,
                                        _ => SolveStatus.Timeout
                                    },
                                    IterationTime = iterTime,
                                    Iteration = iteration
                                };
                                emit = true;
                                break;

                            case WarningOutput o:
                                warnings ??= [];
                                warnings.Add(o.Message);
                                break;

                            case ErrorOutput o:
                                msg = msg with
                                {
                                    TimeStamp = endTime,
                                    TotalTime = totalTime,
                                    Status = o.Kind switch
                                    {
                                        "SyntaxError" => SolveStatus.SyntaxError,
                                        "TypeError" => SolveStatus.TypeError,
                                        "AssertionError" => SolveStatus.AssertionError,
                                        "EvaluationError" => SolveStatus.EvaluationError,
                                        _ => SolveStatus.Error
                                    },
                                    IterationTime = iterTime,
                                    Iteration = iteration,
                                    Error = o.Message
                                };
                                emit = true;
                                break;

                            case SolutionOutput o:

                                string? dzn = null;
                                string? raw = null;
                                if (o.Sections is { } sections)
                                {
                                    foreach (var section in sections)
                                    {
                                        switch (section)
                                        {
                                            case "dzn":
                                                dzn = o.Output[section].ToString();
                                                break;
                                            case "raw":
                                                raw = o.Output[section].ToString();
                                                break;
                                        }
                                    }
                                }

                                iteration++;
                                if (string.IsNullOrWhiteSpace(dzn))
                                {
                                    msg = msg with
                                    {
                                        TimeStamp = endTime,
                                        TotalTime = totalTime,
                                        Status = SolveStatus.Satisfied,
                                        IterationTime = iterTime,
                                        Iteration = iteration,
                                        Output = raw
                                    };
                                }
                                else if (
                                    !Parser.TryParseDataString(
                                        dzn,
                                        out var data,
                                        out var err,
                                        out var trace,
                                        out _
                                    )
                                )
                                {
                                    msg = msg with
                                    {
                                        TimeStamp = endTime,
                                        TotalTime = totalTime,
                                        Status = SolveStatus.Error,
                                        IterationTime = iterTime,
                                        Iteration = iteration,
                                        Error = trace,
                                        Output = raw
                                    };
                                }
                                else
                                {
                                    data.Remove("_objective", out var objective);
                                    msg = msg with
                                    {
                                        TimeStamp = endTime,
                                        TotalTime = totalTime,
                                        Status = SolveStatus.Satisfied,
                                        IterationTime = iterTime,
                                        Iteration = iteration,
                                        Objective = objective,
                                        Data = data,
                                        Output = raw
                                    };
                                }
                                emit = true;
                                break;

                            case StatisticsOutput o:
                                statistics ??= new Dictionary<string, JsonValue>();
                                foreach (KeyValuePair<string, JsonNode?> kv in o.Statistics)
                                {
                                    var name = kv.Key;
                                    var value = kv.Value!.AsValue();
                                    statistics[name] = value;
                                    msg = msg with { Statistics = statistics };
                                }

                                break;

                            case CheckerOutput o:
                                if (o.Output is { } checkerOutput && o.Sections is { } checkerSections)
                                {
                                    StringBuilder? sb = null;
                                    foreach (var section in checkerSections)
                                        if (checkerOutput.TryGetValue(section, out var val))
                                            (sb ??= new StringBuilder()).Append(val);
                                    if (sb is not null)
                                        msg = msg with { Checker = sb.ToString() };
                                }
                                break;

                            case CommentOutput _:
                                break;
                        }

                        if (emit)
                            yield return msg;
                        break;

                    case ProcessStdErr e:
                        (stderr ??= new StringBuilder()).AppendLine(e.Text);
                        break;

                    case ProcessExited exited:
                        if (cancellation.IsCancellationRequested)
                        {
                            msg = msg with { Status = SolveStatus.Cancelled };
                            yield return msg;
                        }
                        else if (
                            (stderr is { Length: > 0 } || exited.ExitCode > 0) && !msg.IsError
                        )
                        {
                            msg = msg with
                            {
                                Error = stderr?.ToString(),
                                Status = SolveStatus.Error
                            };
                            yield return msg;
                        }
                        break;
                }
            }
        }
        finally
        {
            if (File.Exists(modelFile))
            {
                try
                {
                    File.Delete(modelFile);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error deleting temporary model file: {ex}");
                }
            }
            timeoutCts?.Dispose();
        }
    }

    [GeneratedRegex(@"MiniZinc to FlatZinc converter, version (\d).(\d).(\d), build (\d*)")]
    private static partial Regex VersionRegex();

    public override string ToString()
    {
        return $"MiniZinc {_version} (\"{_exe}\")";
    }
}
