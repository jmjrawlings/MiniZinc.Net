namespace MiniZinc.Command;

using System.Diagnostics;
using System.Text;
using System.Threading.Channels;

public sealed class CommandRunner : IDisposable
{
    /// The originating command
    private readonly Command _command;

    /// Current state of the process
    private ProcessStatus _status;

    /// Time the process was started
    private DateTimeOffset _startTime;

    /// Time the process ended if it has ended
    private DateTimeOffset _endTime;

    /// Current elapsed duration or total duration if exted
    private TimeSpan _elapsed => _watch.Elapsed;

    /// The process exit code if it has exited
    private int _exitCode;

    /// If listening, the last ProcessMessage received
    private ProcessMessage _current;

    /// The Id of the process if it ever started
    private int _processId;

    private readonly ProcessStartInfo _startInfo;

    private readonly Process _process;

    private readonly Stopwatch _watch;

    /// If iterating, a channel to implement AsyncEnumerable
    private Channel<ProcessMessage>? _channel;

    private IAsyncEnumerable<ProcessMessage>? _events;

    /// <summary>
    /// Create a process from the given command
    /// </summary>
    internal CommandRunner(in Command command)
    {
        _watch = new Stopwatch();
        _startInfo = new ProcessStartInfo
        {
            FileName = command.Exe,
            Arguments = string.Join(' ', command.Arguments),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        if (command.WorkingDirectory is { } path)
        {
            _startInfo.WorkingDirectory = path;
        }

        _process = new Process();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;
        _process.Exited += OnExit;
        _command = command;
    }

    /// <summary>
    /// Run the process until it either terminates or a cancellation
    /// is requested.
    /// </summary>
    internal async Task<ProcessResult> Run(
        bool captureStdOut = true,
        bool captureStdErr = true,
        CancellationToken cancellation = default
    )
    {
        StringBuilder? stdout = null;
        StringBuilder? stderr = null;

        await foreach (var msg in Watch(cancellation))
        {
            switch (msg.EventType)
            {
                case ProcessEventType.Started:
                    break;
                case ProcessEventType.StdOut:
                    if (captureStdOut)
                        (stdout ??= new StringBuilder()).Append(msg.Content);
                    break;
                case ProcessEventType.StdErr:
                    if (captureStdErr)
                        (stderr ??= new StringBuilder()).Append(msg.Content);
                    break;
                case ProcessEventType.Exited:
                    break;
            }
        }

        var output = stdout?.ToString() ?? string.Empty;
        var error = stderr?.ToString() ?? string.Empty;

        var result = new ProcessResult
        {
            Command = _command.ToString(),
            Status = _status,
            StdOut = output,
            StdErr = error,
            StartTime = _startTime,
            EndTime = _endTime,
            Duration = _elapsed,
            ExitCode = _exitCode
        };
        return result;
    }

    /// <inheritdoc cref="Run(bool,bool,System.Threading.CancellationToken)"/>
    internal async Task<ProcessResult> Run(CancellationToken cancellation = default) =>
        await Run(true, true, cancellation);

    internal ProcessResult WaitSync() => Run(CancellationToken.None).Result;

    /// <summary>
    /// Start the process and consume events until it
    /// terminates
    /// </summary>
    internal IAsyncEnumerable<ProcessMessage> Watch(CancellationToken cancellation = default)
    {
        if (_events is not null)
            return _events;

        if (_status is not ProcessStatus.Idle)
            throw new InvalidOperationException();

        _startTime = DateTimeOffset.Now;
        _endTime = _startTime;
        _status = ProcessStatus.Running;
        _status = ProcessStatus.Running;
        _channel = Channel.CreateUnbounded<ProcessMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );
        _events = _channel.Reader.ReadAllAsync();
        _watch.Start();
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        _processId = _process.Id;
        _current = new ProcessMessage
        {
            ProcessId = _processId,
            EventType = ProcessEventType.Started,
            TimeStamp = _startTime
        };

        if (cancellation.IsCancellationRequested)
            Stop();
        else
            cancellation.Register(Stop, useSynchronizationContext: false);

        return _events;
    }

    private void Stop()
    {
        if (_status is not ProcessStatus.Running)
            return;

        if (_process.HasExited)
            return;

        _status = ProcessStatus.Signalled;
        try
        {
            _process.Kill();
        }
        catch { }
    }

    private void OnOutput(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        var elapsed = _elapsed;
        var msg = new ProcessMessage
        {
            ProcessId = _processId,
            Content = data,
            EventType = ProcessEventType.StdOut,
            TimeStamp = _startTime + elapsed
        };
        _current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnError(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        var elapsed = _elapsed;
        var msg = new ProcessMessage
        {
            ProcessId = _processId,
            Content = e.Data,
            EventType = ProcessEventType.StdErr,
            TimeStamp = _startTime + elapsed
        };
        _current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnExit(object? _s, EventArgs _e)
    {
        _watch.Stop();
        _exitCode = _process.ExitCode;
        switch (_exitCode)
        {
            case 0:
                _status = ProcessStatus.Ok;
                break;
            case { } when _status is ProcessStatus.Signalled:
                _status = ProcessStatus.Cancelled;
                break;
            default:
                _status = ProcessStatus.Error;
                break;
        }

        _current = new ProcessMessage
        {
            ProcessId = _processId,
            EventType = ProcessEventType.Exited,
            TimeStamp = _startTime + _elapsed
        };

        _channel?.Writer.TryWrite(_current);
        _channel?.Writer.TryComplete();
    }

    ///
    public void Dispose()
    {
        Stop();
        _process.Dispose();
    }

    ///
    public override string ToString()
    {
        return $"<Process \"{_command.Exe}\" | {_status} after {_watch.Elapsed.TotalSeconds}s>";
    }
}
