namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Command;
using Core;
using Models;
using Parser;
using Parser.Syntax;

public abstract class SolverProcess<T> : IAsyncEnumerator<T>, IAsyncEnumerable<T>
{
    public readonly Model Model;
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
    private readonly Process _process;
    private readonly Stopwatch _watch;
    private readonly ProcessStartInfo _startInfo;
    private readonly Channel<T> _channel;
    protected readonly List<string> _warnings;
    protected TimePeriod _totalTime;
    protected TimePeriod _iterTime;
    protected int _iteration;
    protected SolveStatus _solveStatus;
    protected Dictionary<string, SyntaxNode> _data;
    private int? _exitCode;
    private ProcessStatus _processStatus;
    private readonly TaskCompletionSource<T> _completion;
    protected Dictionary<string, object>? _statistics;
    private readonly Mutex _mutex;
    protected T? _current;

    internal SolverProcess(
        MiniZincClient client,
        Model model,
        SolveOptions? options = null,
        CancellationToken cancellation = default
    )
    {
        _client = client;
        Model = model;
        Options = options;
        _cancellation = cancellation;
        _completion = new TaskCompletionSource<T>();
        _warnings = new List<string>();
        _mutex = new Mutex();
        _data = new Dictionary<string, SyntaxNode>();
        model.EnsureOk();
        ModelText = model.SourceText;
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

    protected abstract T CreateResult(
        in IntOrFloat? objectiveValue = null,
        in IntOrFloat? objectiveBoundValue = null,
        string? error = null
    );

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

            case StatisticsOutput o:
                OnStatisticsOutput(o);
                break;

            case CommentOutput _:
                break;
        }
        _mutex.ReleaseMutex();
    }

    private void OnStatisticsOutput(StatisticsOutput o)
    {
        foreach (var kv in o.Statistics)
        {
            var name = kv.Key;
            var value = kv.Value?.AsValue();
            if (value is null)
                continue;
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
            _statistics ??= new Dictionary<string, object>();
            _statistics.Add(name, stat);
        }
    }

    private void OnSolutionOutput(SolutionOutput o)
    {
        _iteration++;
        _solveStatus = SolveStatus.Satisfied;
        // TODO - is this always the case?
        var dzn = o.Output["dzn"].ToString();
        var parsed = Parser.ParseString(dzn!);
        _data = new Dictionary<string, SyntaxNode>();
        parsed.EnsureOk();
        foreach (var node in parsed.SyntaxNode.Nodes)
        {
            if (node is not AssignmentSyntax assign)
                throw new Exception();
            var name = assign.Identifier.ToString();
            var value = assign.Expr;
            _data[name] = value;
        }
        _data.TryGetValue("_objective", out var objectiveNode);
        IntOrFloat? objective = objectiveNode switch
        {
            null => null,
            IntLiteralSyntax i => IntOrFloat.Int(i),
            FloatLiteralSyntax f => IntOrFloat.Float((float)f.Value),
            var other => throw new Exception(other.GetType().FullName)
        };
        _current = CreateResult(objectiveValue: objective);
        _channel.Writer.TryWrite(_current);
    }

    private void OnWarningOutput(WarningOutput o)
    {
        _mutex.WaitOne();
        _warnings.Add(o.Message);
        _mutex.ReleaseMutex();
    }

    private void OnErrorOutput(ErrorOutput o)
    {
        _current = CreateResult(error: o.Message);
        _channel.Writer.TryWrite(_current);
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
        _current = CreateResult();
        _channel.Writer.TryWrite(_current);
    }

    private void OnProcessError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not { } warning)
            return;

        _warnings.Add(warning);
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

        if (_current is null)
        {
            _solveStatus = SolveStatus.Error;
            _current = CreateResult(error: $"MiniZinc exited without producing a result");
            _channel.Writer.TryWrite(_current);
        }

        _completion.SetResult(_current);
        _channel.Writer.TryComplete();
        _mutex.ReleaseMutex();
    }

    void Warn(string msg)
    {
        _warnings.Add(msg);
    }

    public TaskAwaiter<T> GetAwaiter() => _completion.Task.GetAwaiter();

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(
        CancellationToken cancellationToken = new CancellationToken()
    ) => this;

    private T? _asyncCurrent;

    async ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
    {
        await _channel.Reader.WaitToReadAsync(_cancellation);
        if (!_channel.Reader.TryRead(out _asyncCurrent))
            return false;
        return true;
    }

    T IAsyncEnumerator<T>.Current => _asyncCurrent!;

    public async ValueTask DisposeAsync()
    {
        Stop();
        try
        {
            _mutex.Dispose();
        }
        catch { }
    }

    public void Dispose()
    {
        Stop();
        _mutex.Dispose();
    }

    public override string ToString() => CommandString;
}

public sealed class SolverProcess : SolverProcess<SolveResult>
{
    public SolverProcess(
        MiniZincClient client,
        Model model,
        SolveOptions? options = null,
        CancellationToken cancellation = default
    )
        : base(client, model, options, cancellation) { }

    protected override SolveResult CreateResult(
        in IntOrFloat? objectiveValue = null,
        in IntOrFloat? objectiveBoundValue = null,
        string? error = null
    )
    {
        var result = new SolveResult
        {
            Command = CommandString,
            ProcessId = ProcessId,
            SolverId = SolverId,
            TotalTime = _totalTime,
            Status = _solveStatus,
            Iteration = _iteration,
            Warnings = _warnings,
            IterationTime = _iterTime,
            Data = _data,
            Statistics = _statistics,
            Error = error,
            Objective = objectiveValue,
            ObjectiveBound = objectiveBoundValue,
            AbsoluteGapToOptimality = default,
            RelativeGapToOptimality = default,
            AbsoluteIterationGap = default,
            RelativeIterationGap = default
        };
        return result;
    }
}

public sealed class IntProcess : SolverProcess<IntResult>
{
    public IntProcess(
        MiniZincClient client,
        Model model,
        SolveOptions? options = null,
        CancellationToken cancellation = default
    )
        : base(client, model, options, cancellation) { }

    protected override IntResult CreateResult(
        in IntOrFloat? objectiveValue = null,
        in IntOrFloat? objectiveBoundValue = null,
        string? error = null
    )
    {
        Guard.IsFalse(objectiveValue?.IsFloat ?? false);
        Guard.IsFalse(objectiveBoundValue?.IsFloat ?? false);
        int? objective = objectiveValue?.IntValue;
        int? objectiveBound = objectiveBoundValue?.IntValue;
        int? absoluteGapToOptimality = objective - objectiveBound;
        double? relativeGapToOptimality = absoluteGapToOptimality / objectiveBound;
        int? absoluteIterationGap = objective - _current?.Objective;
        double? relativeIterationGap = absoluteIterationGap / _current?.Objective;

        var result = new IntResult
        {
            Command = CommandString,
            ProcessId = ProcessId,
            SolverId = SolverId,
            TotalTime = _totalTime,
            Status = _solveStatus,
            Iteration = _iteration,
            Warnings = _warnings,
            IterationTime = _iterTime,
            Data = _data,
            Statistics = _statistics,
            Error = error,
            Objective = objective,
            ObjectiveBound = objectiveBound,
            AbsoluteGapToOptimality = absoluteGapToOptimality,
            RelativeGapToOptimality = relativeGapToOptimality,
            AbsoluteIterationGap = absoluteIterationGap,
            RelativeIterationGap = relativeIterationGap
        };
        return result;
    }
}

public sealed class FloatProcess : SolverProcess<FloatResult>
{
    public FloatProcess(
        MiniZincClient client,
        Model model,
        SolveOptions? options = null,
        CancellationToken cancellation = default
    )
        : base(client, model, options, cancellation) { }

    protected override FloatResult CreateResult(
        in IntOrFloat? objectiveValue = null,
        in IntOrFloat? objectiveBoundValue = null,
        string? error = null
    )
    {
        Guard.IsTrue(objectiveValue?.IsFloat ?? true);
        Guard.IsTrue(objectiveBoundValue?.IsFloat ?? true);
        float? objective = objectiveValue?.FloatValue;
        float? objectiveBound = objectiveBoundValue?.FloatValue;
        float? absoluteGapToOptimality = objective - objectiveBound;
        double? relativeGapToOptimality = absoluteGapToOptimality / objectiveBound;
        float? absoluteIterationGap = objective - _current?.Objective;
        double? relativeIterationGap = absoluteIterationGap / _current?.Objective;

        var result = new FloatResult
        {
            Command = CommandString,
            ProcessId = ProcessId,
            SolverId = SolverId,
            TotalTime = _totalTime,
            Status = _solveStatus,
            Iteration = _iteration,
            Warnings = _warnings,
            IterationTime = _iterTime,
            Data = _data,
            Statistics = _statistics,
            Error = error,
            Objective = objective,
            ObjectiveBound = objectiveBound,
            AbsoluteGapToOptimality = absoluteGapToOptimality,
            RelativeGapToOptimality = relativeGapToOptimality,
            AbsoluteIterationGap = absoluteIterationGap,
            RelativeIterationGap = relativeIterationGap
        };
        return result;
    }
}
