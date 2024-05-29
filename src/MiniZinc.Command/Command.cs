namespace MiniZinc.Command;

using System.Runtime.CompilerServices;
using System.Text;

public readonly struct Command
{
    public readonly string Exe;

    private readonly List<Arg> _args;
    public IReadOnlyList<Arg> Args => _args;

    public readonly string String;

    public Command(string exe, params object[] args)
    {
        if (string.IsNullOrEmpty(exe))
            throw new ArgumentNullException(exe);
        Exe = exe;
        _args = new List<Arg>();
        var sb = new StringBuilder();
        sb.Append(exe);
        foreach (var arg in Arg.Parse(args))
        {
            _args.Add(arg);
            sb.Append(' ');
            sb.Append(arg.String);
        }
        String = sb.ToString();
    }

    public Command Add(params object[] args)
    {
        var cmd = new Command(Exe, Args, args);
        return cmd;
    }

    public override string ToString()
    {
        return String;
    }

    public async Task<ProcessResult> Run(
        string? workingDir = null,
        bool captureStdout = true,
        bool captureStderr = true,
        CancellationToken cancellation = default
    )
    {
        using var proc = new CommandRunner(this, workingDir);
        var result = await proc.Run(
            captureStdErr: captureStderr,
            captureStdOut: captureStdout,
            cancellation: cancellation
        );
        return result;
    }

    public async IAsyncEnumerable<ProcessMessage> Watch(
        string? workingDir = null,
        [EnumeratorCancellation] CancellationToken cancellation = default
    )
    {
        using var proc = new CommandRunner(this, workingDir);
        await foreach (var msg in proc.Watch(cancellation))
            yield return msg;
    }
}
