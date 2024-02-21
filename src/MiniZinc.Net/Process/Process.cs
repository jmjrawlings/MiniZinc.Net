namespace MiniZinc.Net.Process;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SystemProcess = System.Diagnostics.Process;

/// <summary>
/// Wraps and starts a System.Diagnostics.Process
/// </summary>
public sealed class Process : IDisposable, IAsyncEnumerable<ProcessMessage>
{
    /// The originating command
    public readonly Command Command;

    /// Current state of the process
    public ProcessState State { get; private set; }

    /// Time the process was started
    public DateTime StartTime => _process.StartTime;

    /// Current elapsed duration or total duration if exted
    public TimeSpan Elapsed => _watch.Elapsed;

    /// Current timestamp or the completion time if exited
    public DateTime TimeStamp => StartTime + Elapsed;

    /// The process exit code if it has exited
    public int ExitCode { get; private set; }

    /// If listening, the last ProcessMessage received
    public ProcessMessage LastMessage { get; private set; }

    /// The Id of the process if it ever started
    public int ProcessId { get; private set; }

    /// If captured, everything from stderr
    private readonly StringBuilder? _stderr;

    /// If captured, everything from stdout
    private readonly StringBuilder? _stdout;

    /// If listening, a channel to implement AsyncEnumerable
    private Channel<ProcessMessage>? _channel;

    /// If listening, a stream of messages
    private IAsyncEnumerable<ProcessMessage>? _messages;

    private readonly ProcessStartInfo _startInfo;
    private readonly SystemProcess _process;
    private readonly Stopwatch _watch;
    private readonly CancellationToken _cancellationToken;
    private readonly TaskCompletionSource<ProcessResult> _tcs;
    private readonly ILogger _logger;

    public Process(
        in Command command,
        bool stderr = true,
        bool stdout = true,
        CancellationToken cancellationToken = default
    )
    {
        _logger = Logging.Factory.CreateLogger<Process>();
        Command = command;
        _cancellationToken = cancellationToken;
        _watch = new Stopwatch();

        if (stderr)
            _stderr = new StringBuilder();

        if (stdout)
            _stdout = new StringBuilder();

        _tcs = new TaskCompletionSource<ProcessResult>();
        _startInfo = new ProcessStartInfo
        {
            FileName = command.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogInformation("Command is {Command}", command.String);
        foreach (var arg in command.Arguments)
        {
            _logger.LogDebug("{Arg}", arg);
            _startInfo.ArgumentList.Add(arg.String);
        }

        if (command.WorkingDir is { } path)
        {
            _logger.LogInformation("Setting working directory to {Path}", path);
            _startInfo.WorkingDirectory = path;
        }

        _process = new SystemProcess();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;
        _process.Exited += OnExit;
        _cancellationToken.Register(OnCancel);
    }

    /// <summary>
    /// The result of the Process
    /// </summary>
    public Task<ProcessResult> Result
    {
        get
        {
            if (State is ProcessState.Idle)
                Start();
            return _tcs.Task;
        }
    }

    private void Start()
    {
        _logger.LogInformation("Starting process");
        State = ProcessState.Running;
        _watch.Start();
        _process.Start();
        ProcessId = _process.Id;
        LastMessage = new ProcessMessage
        {
            Command = Command.String,
            StartTime = StartTime,
            ProcessId = ProcessId,
            MessageType = ProcessMessageType.Started,
            TimeStamp = StartTime
        };
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _logger.LogInformation("ProcessId is {ProcessId}", _process.Id);
        _channel?.Writer.TryWrite(LastMessage);
    }

    private void OnOutput(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        _logger.LogDebug("{Output}", data);
        _stdout?.Append(data);

        var elapsed = Elapsed;
        var msg = LastMessage with
        {
            Content = data,
            MessageType = ProcessMessageType.StdOut,
            TimeStamp = StartTime + elapsed,
            Elapsed = elapsed
        };
        LastMessage = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnError(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        _logger.LogError("{Error}", data);
        _stderr?.Append(data);

        var elapsed = Elapsed;
        var msg = LastMessage with
        {
            Content = e.Data,
            MessageType = ProcessMessageType.StdErr,
            TimeStamp = StartTime + elapsed,
            Elapsed = elapsed
        };
        LastMessage = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnCancel()
    {
        if (State is not ProcessState.Running)
            return;

        if (_process.HasExited)
            return;

        _logger.LogInformation("Cancellation requested - killing process");
        State = ProcessState.Signalled;
        _process.Kill();
    }

    private void OnExit(object? _s, EventArgs _e)
    {
        _watch.Stop();
        LogLevel level;
        switch (_process.ExitCode)
        {
            case 0:
                level = LogLevel.Information;
                State = ProcessState.Ok;
                break;
            case { } when State is ProcessState.Cancelled:
                level = LogLevel.Warning;
                State = ProcessState.Cancelled;
                break;
            case var code:
                level = LogLevel.Error;
                State = ProcessState.Error;
                break;
        }
        ExitCode = _process.ExitCode;
        _logger.Log(
            level,
            "Process exited with code {ExitCode} after {Elapsed}",
            ExitCode,
            Elapsed
        );

        var result = new ProcessResult
        {
            Command = Command.String,
            State = State,
            StartTime = StartTime,
            EndTime = TimeStamp,
            Duration = Elapsed,
            ExitCode = _process.ExitCode,
            StdOut = _stdout?.ToString(),
            StdErr = _stderr?.ToString()
        };
        _tcs.SetResult(result);

        LastMessage = LastMessage with
        {
            MessageType = ProcessMessageType.Exited,
            Content = null,
            TimeStamp = StartTime + Elapsed,
            Elapsed = Elapsed
        };

        if (_channel is null)
            return;

        _channel.Writer.TryWrite(LastMessage);
        _channel.Writer.TryComplete();
    }

    public IAsyncEnumerator<ProcessMessage> GetAsyncEnumerator(
        CancellationToken cancellationToken = default
    )
    {
        if (_messages is not null)
            return _messages.GetAsyncEnumerator(cancellationToken);

        _channel = Channel.CreateUnbounded<ProcessMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false
            }
        );
        _messages = _channel.Reader.ReadAllAsync(_cancellationToken);

        if (State is ProcessState.Idle)
            Start();

        return _messages.GetAsyncEnumerator(cancellationToken);
    }

    public void Dispose()
    {
        _process.Dispose();
    }

    public override string ToString()
    {
        return $"<{Command.Exe} | {State} | {_watch.Elapsed}>";
    }
}
