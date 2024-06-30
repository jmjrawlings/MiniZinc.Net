namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Command;
using Compiler;
using Core;
using Parser;
using Parser.Syntax;

public abstract class SolverProcess<T> : IAsyncEnumerable<T>
{
    public readonly Model Model;
    public readonly SolveOptions? Options;
    public readonly Command Command;
    public readonly string CommandString;
    public int ProcessId { get; private set; }
    private readonly CancellationToken _cancellation;
    public readonly Solver Solver;
    public readonly string SolverId;
    public readonly string ModelText;
    public readonly DirectoryInfo ModelFolder;
    public readonly FileInfo ModelFile;
    public readonly string ModelPath;
    private readonly MiniZincClient _client;
    private readonly Process _process;
    private readonly Stopwatch _watch;
    private readonly ProcessStartInfo _startInfo;
    private readonly Channel<T> _channel;
    private readonly Task _task;
    protected readonly List<string> _warnings;
    protected TimePeriod _totalTime;
    protected TimePeriod _iterTime;
    protected int _iteration;
    protected SolveStatus _solveStatus;
    protected DataSyntax _data;
    protected string? _dataString;
    private int? _exitCode;
    private ProcessStatus _processStatus;
    private readonly TaskCompletionSource<T> _completion;
    protected Dictionary<string, object>? _statistics;
    protected T? _current;
    private readonly IAsyncEnumerator<T> _asyncEnumerator;

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
        _process.StartInfo = _startInfo;
        _processStatus = ProcessStatus.Idle;
        _channel = Channel.CreateUnbounded<T>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );
        _asyncEnumerator = _channel.Reader.ReadAllAsync(_cancellation).GetAsyncEnumerator();
        _task = Task.Factory.StartNew(Run);
    }

    private void Run()
    {
        _process.Start();
        ProcessId = _process.Id;

        while (!_process.StandardOutput.EndOfStream)
        {
            string? msg = _process.StandardOutput.ReadLine();
            if (msg is null)
                break;

            var time = DateTimeOffset.Now;
            _totalTime = _totalTime.WithEnd(time);
            _iterTime = _iterTime.WithEnd(time);
            var output = JsonOutput.Deserialize(msg);
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
        }

        if (!_process.HasExited)
            _process.WaitForExit();

        _watch.Stop();
        _exitCode = _process.ExitCode;
        if (_exitCode is 0)
            _processStatus = ProcessStatus.Ok;
        else
            _processStatus = ProcessStatus.Error;

        if (_current is null)
        {
            _solveStatus = SolveStatus.Error;
            var error = _process.StandardError.ReadToEnd();
            if (string.IsNullOrEmpty(error))
                _current = CreateResult(error: $"MiniZinc exited without producing a result");
            else
                _current = CreateResult(error: error);
        }
        _channel.Writer.TryWrite(_current);
        _channel.Writer.Complete();
        _completion.SetResult(_current);
    }

    protected abstract T CreateResult(
        in IntOrFloat? objectiveValue = null,
        in IntOrFloat? objectiveBoundValue = null,
        string? error = null
    );

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
        if (o.Sections is not { } sections)
            return;
        _dataString = null;
        foreach (var section in sections)
        {
            if (section is "dzn")
                _dataString = o.Output[section].ToString();
        }

        if (_dataString is not null)
        {
            var parsed = Parser.ParseDataString(_dataString, out _data);
            parsed.EnsureOk();
            if (_data.TryGetValue("_objective", out var objectiveNode))
                _data.Remove("_objective");

            IntOrFloat? objective = objectiveNode switch
            {
                null => null,
                IntLiteralSyntax i => IntOrFloat.Int(i),
                FloatLiteralSyntax f => IntOrFloat.Float((float)f.Value),
                var other => throw new Exception(other.GetType().FullName)
            };
            _current = CreateResult(objectiveValue: objective);
        }
        else
        {
            _current = CreateResult();
        }
        _channel.Writer.TryWrite(_current);
    }

    private void OnWarningOutput(WarningOutput o)
    {
        _warnings.Add(o.Message);
    }

    private void OnErrorOutput(ErrorOutput o)
    {
        _solveStatus = o.Kind switch
        {
            "SyntaxError" => SolveStatus.SyntaxError,
            "TypeError" => SolveStatus.TypeError,
            "AssertionError" => SolveStatus.AssertionError,
            "EvaluationError" => SolveStatus.EvaluationError,
            _ => SolveStatus.Error
        };
        _current = CreateResult(error: o.Message);
        _channel.Writer.TryWrite(_current);
    }

    private void OnStatusOutput(StatusOutput o)
    {
        SolveStatus? solveStatus = o.Status switch
        {
            "ALL_SOLUTIONS" => SolveStatus.AllSolutions,

            "OPTIMAL_SOLUTION" => SolveStatus.Optimal,

            "UNSATISFIABLE" => SolveStatus.Unsatisfiable,

            "UNBOUNDED" => SolveStatus.Unbounded,

            "UNSAT_OR_UNBOUNDED" => SolveStatus.UnsatOrUnbounded,

            "ERROR" => SolveStatus.Error,

            _ => null
        };

        if (solveStatus is { } status)
        {
            _solveStatus = status;
            _current = CreateResult();
        }
        else
        {
            // TODO - confirm
            _solveStatus = SolveStatus.Timeout;
            _current = CreateResult(error: "time limit reached");
        }

        _channel.Writer.TryWrite(_current);
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
            _channel.Writer.TryComplete();
            _completion.SetResult(_current);
        }
    }

    void Warn(string msg)
    {
        _warnings.Add(msg);
    }

    public TaskAwaiter<T> GetAwaiter() => _completion.Task.GetAwaiter();

    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(
        CancellationToken cancellationToken = new CancellationToken()
    ) => _asyncEnumerator;

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
            // DataString = _dataString,
            Data = _data,
            Statistics = _statistics,
            Error = error,
            Objective = objectiveValue ?? _current?.Objective,
            ObjectiveBound = objectiveBoundValue ?? _current?.ObjectiveBound,
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
        int? objectivePrev = _current?.Objective;
        int? objective = objectiveValue?.IntValue ?? _current?.Objective;
        int? objectiveBoundPrev = _current?.ObjectiveBound;
        int? objectiveBound = objectiveBoundValue?.IntValue ?? _current?.ObjectiveBound;
        int? absoluteGapToOptimality = objective - objectiveBound;
        int? absoluteIterationGap = objective - objectivePrev;
        double? relativeGapToOptimality = null;
        double? relativeIterationGap = null;

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
            // DataString = _dataString,
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
        float? objectivePrev = _current?.Objective;
        float? objective = objectiveValue?.FloatValue ?? objectivePrev;
        float? objectiveBoundPrev = _current?.ObjectiveBound;
        float? objectiveBound = objectiveBoundValue?.FloatValue ?? objectiveBoundPrev;
        float? absoluteGapToOptimality = objective - objectiveBound;
        float? absoluteIterationGap = objective - _current?.Objective;
        double? relativeGapToOptimality = null; //absoluteGapToOptimality / objectiveBound;
        double? relativeIterationGap = null; // absoluteIterationGap / _current?.Objective;
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
            // DataString = _dataString,
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
