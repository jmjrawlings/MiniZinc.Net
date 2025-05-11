namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Channels;
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
        var cmd = new Command($"\"{_exe.FullName}\"");
        cmd.AddArgs(args);
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

    public async Task<MiniZincMessage> Solution(
        MiniZincModel model,
        string? solver = null,
        CancellationToken token = default,
        params string?[] args
    )
    {
        MiniZincMessage? msg = null;
        await foreach (var message in Solve(model, solver, token, args))
        {
            msg = message;
        }

        if (msg is null)
            throw new Exception($"No message returned");

        return msg;
    }

    public async IAsyncEnumerable<MiniZincMessage> Solve(
        MiniZincModel model,
        string? solver = null,
        [EnumeratorCancellation] CancellationToken token = default,
        params string?[] args
    )
    {
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

        var directory = Path.GetTempPath();
        string modelString = model.Write();
        string modelFile = Path.Join(
            directory,
            $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.mzn"
        );

        await File.WriteAllTextAsync(modelFile, modelString, token);
        var command = Command();
        command.AddArgs(args);
        foreach (var arg in command.Arguments.Values)
            if (arg.Flag?.Equals("solver") ?? false)
                if (solver is not null)
                    throw new ArgumentException(
                        $"Solver was provided both as an argument and command line"
                    );
                else
                    solver = arg.Value;
        command.AddArgs("--json-stream", "--output-objective", "--statistics");
        solver ??= MiniZincSolver.GECODE;
        var solverInfo = GetSolver(solver);
        command.AddArgs(modelFile);
        command.AddArgs($"--solver {solverInfo.Id}");
        var commandString = command.ToString();

        var startInfo = new ProcessStartInfo
        {
            FileName = command.Exe,
            Arguments = string.Join(' ', command.Arguments),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true
        };

        if (command.WorkingDirectory is { } path)
            startInfo.WorkingDirectory = path;

        Channel<MiniZincMessage> channel = Channel.CreateUnbounded<MiniZincMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );

        Task processTask = Task.Run(async () =>
        {
            using Process process = new Process();
            process.StartInfo = startInfo;
            Dictionary<string, object>? statistics = null;
            List<string>? warnings = null;
            int iteration = 0;
            DateTimeOffset startTime = DateTimeOffset.Now;
            DateTimeOffset lastTime = DateTimeOffset.Now;
            DateTimeOffset endTime = DateTimeOffset.Now;
            TimeSpan iterTime = TimeSpan.Zero;
            TimeSpan totalTime = TimeSpan.Zero;
            token.Register(() =>
            {
                if (!process.HasExited)
                    process.Kill();
            });
            MiniZincMessage msg = new MiniZincMessage
            {
                Command = commandString,
                Model = modelString,
                Solver = solverInfo,
                TimeStamp = startTime
            };

            try
            {
                process.Start();

                msg = msg with { ProcessId = process.Id };

                while (await process.StandardOutput.ReadLineAsync(token) is { } line)
                {
                    endTime = DateTimeOffset.Now;
                    iterTime = endTime - lastTime;
                    totalTime = endTime - startTime;
                    lastTime = endTime;
                    JsonOutput output = JsonOutput.Deserialize(line);
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
                            channel.Writer.TryWrite(msg);
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
                            channel.Writer.TryWrite(msg);
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
                            if (dzn is null)
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
                                    out var token
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
                            channel.Writer.TryWrite(msg);
                            break;

                        case StatisticsOutput o:
                            statistics ??= new Dictionary<string, object>();
                            foreach (KeyValuePair<string, JsonNode?> kv in o.Statistics)
                            {
                                var name = kv.Key;
                                var value = kv.Value!.AsValue();
                                object? stat = null;
                                if (value.TryGetValue<int>(out var i))
                                    stat = i;
                                else if (value.TryGetValue<bool>(out var b))
                                    stat = b;
                                else if (value.TryGetValue<float>(out var f))
                                    stat = f;
                                else if (value.TryGetValue<string>(out var s))
                                    stat = s;
                                if (stat is null)
                                    continue;
                                statistics[name] = stat;
                                msg = msg with { Statistics = statistics };
                            }

                            break;

                        case CommentOutput _:
                            break;
                    }
                }

                string stderr = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                int exitCode = process.ExitCode;

                if (!string.IsNullOrEmpty(stderr) || exitCode > 0)
                {
                    if (!msg.IsError)
                    {
                        msg = msg with { Error = stderr, Status = SolveStatus.Error };
                        channel.Writer.TryWrite(msg);
                    }
                }
            }
            catch (OperationCanceledException exn)
            {
                if (!process.HasExited)
                    process.Kill();
                msg = msg with { Status = SolveStatus.Cancelled };
                channel.Writer.TryWrite(msg);
            }
            catch (Exception exn)
            {
                if (!process.HasExited)
                    process.Kill();
                msg = msg with { Status = SolveStatus.Error, Error = exn.Message };
                channel.Writer.TryWrite(msg);
            }
            finally
            {
                channel.Writer.Complete();
            }
        });

        await foreach (var message in channel.Reader.ReadAllAsync())
        {
            yield return message;
        }

        try
        {
            await processTask;
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
        }
    }

    [GeneratedRegex(@"MiniZinc to FlatZinc converter, version (\d).(\d).(\d), build (\d*)")]
    private static partial Regex VersionRegex();

    public override string ToString()
    {
        return $"MiniZinc {_version} (\"{_exe}\")";
    }
}
