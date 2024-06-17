namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Command;
using FileSystem;
using Parser;
using Parser.Syntax;

public abstract class MiniZincProcess<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
    where T : MiniZincResult
{
    public readonly MiniZincModel MiniZincModel;
    public readonly SolveOptions? Options;
    public readonly Command Command;
    public readonly string CommandString;
    private readonly CancellationToken _cancellation;
    public readonly Solver Solver;
    public readonly string SolverId;
    public readonly string ModelText;
    public readonly DirectoryInfo ModelFolder;
    public readonly FileInfo ModelFile;
    public readonly string ModelPath;
    public readonly int ProcessId;

    private readonly MiniZincClient _client;
    private readonly List<string> _warnings;
    private readonly Process _process;
    private readonly Stopwatch _watch;
    private readonly ProcessStartInfo _startInfo;
    private readonly Channel<T> _channel;
    private TimePeriod _totalTime;
    private TimePeriod _iterTime;
    private T _prev;
    private T _current;
    private int _iteration;
    private SolveStatus _solveStatus;
    private SyntaxNode? _objective;
    private int? _exitCode;
    private ProcessStatus _processStatus;
    private readonly TaskCompletionSource<T> _completion;
    private readonly Mutex _mutex;

    internal MiniZincProcess(
        MiniZincClient client,
        MiniZincModel miniZincModel,
        SolveOptions? options = null,
        CancellationToken cancellation = default
    )
    {
        _client = client;
        MiniZincModel = miniZincModel;
        Options = options;
        _cancellation = cancellation;
        _completion = new TaskCompletionSource<T>();
        _warnings = new List<string>();
        _mutex = new Mutex();
        miniZincModel.EnsureOk();
        ModelText = miniZincModel.SourceText;
        SolverId = options?.SolverId ?? Solver.Gecode;
        Solver = _client.GetSolver(SolverId);
        ModelFolder = new DirectoryInfo(options?.OutputFolder ?? Path.GetTempPath());
        ModelFile = ModelFolder.JoinFile(Path.ChangeExtension(Path.GetTempFileName(), ".mzn"));
        ModelPath = ModelFile.FullName;
        File.WriteAllText(ModelPath, ModelText);
        Command = _client.Command("--solver", Solver.Id, "--json-stream", "--output-objective");
        var timeout = options?.Timeout;
        if (options?.Arguments is { } args)
        {
            foreach (var arg in args)
            {
                switch (arg.Flag)
                {
                    case null:
                        break;
                    case "timeout":
                        if (timeout.HasValue)
                            Warn("Discarding command line arg \"timeout\"");
                        else if (arg.Value is not { } val)
                            break;
                        else
                            timeout = TimeSpan.FromMilliseconds(int.Parse(val));
                        break;
                    case "solver":
                        Warn("Discarding command line arg \"solver\"");
                        break;
                    default:
                        Command = Command.AddArgs(arg);
                        break;
                }
            }
        }

        if (timeout.HasValue)
            Command = Command.AddArgs("--time-limit", timeout.Value.TotalMilliseconds.ToString());

        Command = Command.AddArgs(ModelPath);
        CommandString = Command.ToString();

        _watch = Stopwatch.StartNew();
        _totalTime = new TimePeriod(DateTimeOffset.Now);
        _iterTime = _totalTime;
        _startInfo = new ProcessStartInfo
        {
            FileName = Command.Exe,
            Arguments = string.Join(' ', Command.Arguments),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (Command.WorkingDirectory is { } path)
        {
            _startInfo.WorkingDirectory = path;
        }
        _process = new Process();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnProcessOutput;
        _process.ErrorDataReceived += OnProcessError;
        _process.Exited += OnProcessExited;
        _processStatus = ProcessStatus.Idle;
        _channel = Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );
        if (_cancellation.IsCancellationRequested)
            return;

        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        ProcessId = _process.Id;
        _cancellation.Register(Stop, useSynchronizationContext: false);
    }

    private void OnProcessOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not { } payload)
            return;

        _mutex.WaitOne();
        var time = DateTimeOffset.Now;
        _totalTime = _totalTime.WithEnd(time);
        _iterTime = _iterTime.WithEnd(time);
        var output = JsonOutput.Deserialize(payload);
        switch (output)
        {
            case StatusOutput o:
                OnStatusOutput(o);
                break;

            case WarningOutput o:
                OnWarningOutput(o);
                break;

            case ErrorOutput o:
                OnErrorOutput(o);
                break;

            case SolutionOutput o:
                OnSolutionOutput(o);
                break;

            case StatOutput _:
                break;

            case CommentOutput _:
                break;
        }
        _mutex.ReleaseMutex();
    }

    private void OnSolutionOutput(SolutionOutput o)
    {
        _iteration++;
        _solveStatus = SolveStatus.Satisfied;
        var dzn = o.Output["dzn"].ToString();
        var parsed = Parser.ParseString(dzn);
        var data = new Dictionary<string, SyntaxNode>();
        parsed.EnsureOk();
        foreach (var node in parsed.SyntaxNode.Nodes)
        {
            if (node is not AssignmentSyntax assign)
                throw new Exception();
            var name = assign.Name.ToString();
            var value = assign.Expr;
            data[name] = value;
        }

        data.TryGetValue("_objective", out _objective);

        switch (_objective)
        {
            case null:
                Publish(
                    new MiniZincSolution
                    {
                        Command = CommandString,
                        ProcessId = ProcessId,
                        SolverId = SolverId,
                        TotalTime = _totalTime,
                        Status = _solveStatus,
                        Data = data,
                        Iteration = _iteration,
                        Warnings = _warnings,
                        IterationTime = _iterTime
                    }
                );
                break;
            case IntLiteralSyntax { Value: var intObj }:

                int? intAbsDelta = null;
                float? relDelta = null;

                if (_prev is IntMiniZincSolution intPrev)
                {
                    intAbsDelta = intObj = intPrev.Objective;
                    relDelta = intAbsDelta / intPrev.Objective;
                }

                Publish(
                    new IntMiniZincSolution
                    {
                        Command = CommandString,
                        ProcessId = ProcessId,
                        SolverId = SolverId,
                        TotalTime = _totalTime,
                        Status = _solveStatus,
                        Data = data,
                        Iteration = _iteration,
                        Warnings = _warnings,
                        IterationTime = _iterTime,
                        Objective = intObj,
                        AbsoluteDelta = intAbsDelta,
                        RelativeDelta = relDelta
                    }
                );
                break;

            case FloatLiteralSyntax { Value: var d }:
                var dblObj = (double)d;
                double? dblAbsDelta = null;
                double? dblRelDelta = null;
                if (_prev is FloatMiniZincSolution prev)
                {
                    dblAbsDelta = dblObj - prev.Objective;
                    dblRelDelta = dblAbsDelta / prev.Objective;
                }

                Publish(
                    new FloatMiniZincSolution
                    {
                        Command = CommandString,
                        ProcessId = ProcessId,
                        SolverId = SolverId,
                        TotalTime = _totalTime,
                        Status = _solveStatus,
                        Data = data,
                        Iteration = _iteration,
                        Warnings = _warnings,
                        IterationTime = _iterTime,
                        Objective = dblObj,
                        AbsoluteDelta = dblAbsDelta,
                        RelativeDelta = dblRelDelta
                    }
                );
                break;

            default:
                throw new Exception($"Unsupported objective {_objective}");
        }
    }

    private void OnWarningOutput(WarningOutput o)
    {
        _mutex.WaitOne();
        _warnings.Add(o.Message);
        _mutex.ReleaseMutex();
    }

    private void OnErrorOutput(ErrorOutput o)
    {
        var kind = o.Kind;
        Publish(
            new MiniZincResult
            {
                Command = CommandString,
                ProcessId = ProcessId,
                SolverId = Solver.Id,
                TotalTime = _totalTime,
                Status = SolveStatus.Error,
                Text = o.Message
            }
        );
    }

    private void OnStatusOutput(StatusOutput o)
    {
        _solveStatus = o.Status switch
        {
            "ALL_SOLUTIONS" => SolveStatus.AllSolutions,

            "OPTIMAL_SOLUTION" => SolveStatus.Optimal,

            "UNSATISFIABLE" => SolveStatus.Unsatisfiable,

            "UNBOUNDED" => SolveStatus.Unbounded,

            "UNSAT_OR_UNBOUNDED" => SolveStatus.UnsatOrUnbounded,

            "UNKNOWN" => SolveStatus.Error,

            "ERROR" => SolveStatus.Error,

            var s => throw new Exception($"Unexpected status {s}")
        };
        Publish(_prev with { TotalTime = _totalTime, Status = _solveStatus });
    }

    private void OnProcessError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not { } warning)
            return;

        _warnings.Add(warning);
    }

    /// <summary>
    /// Publish a the given update
    /// </summary>
    /// <param name="miniZincResult"></param>
    private void Publish(MiniZincResult miniZincResult)
    {
        _prev = miniZincResult;
        _channel.Writer.TryWrite(miniZincResult);
    }

    void Stop()
    {
        if (_processStatus is not ProcessStatus.Running)
            return;

        if (_process.HasExited)
            return;

        _processStatus = ProcessStatus.Signalled;
        try
        {
            _process.Kill();
        }
        catch { }
    }

    private void OnProcessExited(object? sender, EventArgs e)
    {
        _mutex.WaitOne();
        _watch.Stop();
        _exitCode = _process.ExitCode;
        if (_exitCode is 0)
            _processStatus = ProcessStatus.Ok;
        else
            _processStatus = ProcessStatus.Error;
        _channel.Writer.TryComplete();
        _completion.SetResult(_prev);
        _mutex.ReleaseMutex();
        _mutex.Dispose();
    }

    void Warn(string msg)
    {
        _warnings.Add(msg);
    }

    public TaskAwaiter<MiniZincResult<T>> GetAwaiter() => _completion.Task.GetAwaiter();

    IAsyncEnumerator<MiniZincResult<T>> IAsyncEnumerable<MiniZincResult>.GetAsyncEnumerator(
        CancellationToken cancellationToken = new CancellationToken()
    ) => this;

    async ValueTask<bool> IAsyncEnumerator<MiniZincResult<T>>.MoveNextAsync()
    {
        await _channel.Reader.WaitToReadAsync(_cancellation);
        if (!_channel.Reader.TryRead(out _current))
            return false;
        return true;
    }

    MiniZincResult IAsyncEnumerator<MiniZincResult>.Current => _current;

    public async ValueTask DisposeAsync()
    {
        Stop();
    }

    public void Dispose()
    {
        Stop();
    }

    public override string ToString() => CommandString;
}
