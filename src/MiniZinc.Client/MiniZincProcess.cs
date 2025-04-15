namespace MiniZinc.Client;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Threading.Channels;
using Command;
using Core;
using Parser;

public sealed class MiniZincProcess
{
    public readonly MiniZincModel Model;
    public readonly Command Command;
    public readonly string CommandString;
    public readonly MiniZincSolver MiniZincSolver;
    public readonly string? SolverId;
    public readonly string ModelText;
    public readonly DirectoryInfo Directory;
    public readonly FileInfo ModelFile;
    public readonly Arg[]? ExtraArgs;
    private readonly MiniZincClient _client;
    private readonly Process _process;
    private readonly Stopwatch _watch;
    private readonly ProcessStartInfo _startInfo;
    private readonly Channel<MiniZincResult> _channel;
    private List<string>? _warnings;
    private TimePeriod _totalTime;
    private TimePeriod _iterationTime;
    private int _iteration;
    private SolveStatus _solveStatus;
    private MiniZincData _data;
    private string? _dataString;
    private int? _exitCode;
    private ProcessStatus _processStatus;
    private readonly TaskCompletionSource<MiniZincResult> _result;
    private Dictionary<string, object>? _statistics;
    private MiniZincResult? _current;
    public int ProcessId { get; private set; }

    internal MiniZincProcess(
        MiniZincClient client,
        MiniZincModel model,
        string? solverId = null,
        string? directory = null,
        Arg[]? args = null
    )
    {
        _client = client;
        Model = model;
        _result = new TaskCompletionSource<MiniZincResult>();
        _data = new MiniZincData();
        ModelText = model.Write();
        Directory = new DirectoryInfo(directory ?? Path.GetTempPath());
        ModelFile = Directory.JoinFile(Path.ChangeExtension(Path.GetTempFileName(), ".mzn"));
        File.WriteAllText(ModelFile.FullName, ModelText);
        Command = _client.Command("--json-stream", "--output-objective");
        ExtraArgs = args;
        SolverId = solverId;

        if (ExtraArgs is not null)
        {
            Command = Command.AddArgs(ExtraArgs);
            foreach (var arg in ExtraArgs)
                if (arg.Flag?.Equals("solver") ?? false)
                    if (SolverId is not null)
                        throw new ArgumentException(
                            $"Solver was provided both as an argument and command line"
                        );
                    else
                        SolverId = arg.Value;
        }

        SolverId ??= MiniZincSolver.GECODE;
        MiniZincSolver = _client.GetSolver(SolverId);
        Command = Command.AddArgs(ModelFile.FullName);
        CommandString = Command.ToString();

        _watch = Stopwatch.StartNew();
        _totalTime = new TimePeriod(DateTimeOffset.Now);
        _iterationTime = _totalTime;
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
            _startInfo.WorkingDirectory = path;

        _process = new Process();
        _process.StartInfo = _startInfo;
        _processStatus = ProcessStatus.Idle;
        _channel = Channel.CreateUnbounded<MiniZincResult>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );
    }

    /// Starts the process and returns the last solution provided by the solver
    public async Task<MiniZincResult> Solution(CancellationToken token = default)
    {
        await Task.Run(Start, token);
        var result = await _result.Task;
        return result;
    }

    public async IAsyncEnumerable<MiniZincResult> Solutions(
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        var task = Task.Run(Start, token);
        await foreach (MiniZincResult solution in _channel.Reader.ReadAllAsync(token))
        {
            yield return solution;
        }
        await task;
    }

    /// Start the process and wait for it to exit
    private void Start()
    {
        _process.Start();
        ProcessId = _process.Id;

        while (true)
        {
            string? msg = _process.StandardOutput.ReadLine();
            if (msg is null)
                break;

            var time = DateTimeOffset.Now;
            _totalTime = _totalTime.WithEnd(time);
            _iterationTime = _iterationTime.WithEnd(time);
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
                _current = Result(error: $"MiniZinc exited without producing a result");
            else
                _current = Result(error: error);
        }
        _channel.Writer.TryWrite(_current);
        _channel.Writer.Complete();
        _result.SetResult(_current);
    }

    private void OnStatisticsOutput(StatisticsOutput o)
    {
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
            _statistics ??= new Dictionary<string, object>();
            _statistics.Add(name, stat);
        }
    }

    private void OnSolutionOutput(SolutionOutput o)
    {
        _iteration++;
        _solveStatus = SolveStatus.Satisfied;
        _dataString = null;
        if (o.Sections is not { } sections)
        {
            _current = Result();
            goto send;
        }

        foreach (var section in sections)
        {
            if (section is "dzn")
                _dataString = o.Output[section].ToString();
        }

        if (_dataString is null)
        {
            _current = Result();
            goto send;
        }

        var parsed = Parser.ParseDataFromString(_dataString, out _data);
        if (!parsed.Ok)
        {
            _current = Result(error: parsed.ErrorTrace);
            goto send;
        }

        _data.TryGetValue("_objective", out var objectiveNode);
        switch (objectiveNode)
        {
            case null:
                _current = Result();
                break;

            case var data:
                _current = Result(objectiveValue: data);
                break;
        }

        send:
        _channel.Writer.TryWrite(_current);
    }

    private void OnWarningOutput(WarningOutput o)
    {
        Warn(o.Message);
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
        _current = Result(error: o.Message);
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
            _current = Result();
        }
        else
        {
            // TODO - confirm
            _solveStatus = SolveStatus.Timeout;
            _current = Result(error: "time limit reached");
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
            _current = Result(error: $"MiniZinc exited without producing a result");
            _channel.Writer.TryWrite(_current);
            _channel.Writer.TryComplete();
            _result.SetResult(_current);
        }
    }

    void Warn(string msg)
    {
        _warnings ??= [];
        _warnings.Add(msg);
    }

    private MiniZincResult Result(
        in MiniZincExpr? objectiveValue = null,
        in MiniZincExpr? objectiveBoundValue = null,
        string? error = null
    )
    {
        var result = new MiniZincResult
        {
            Command = CommandString,
            ProcessId = ProcessId,
            SolverId = SolverId,
            TotalTime = _totalTime,
            Status = _solveStatus,
            Iteration = _iteration,
            Warnings = _warnings,
            IterationTime = _iterationTime,
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

    public async ValueTask DisposeAsync()
    {
        Stop();
        await ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        Stop();
    }

    public override string ToString() => CommandString;
}
