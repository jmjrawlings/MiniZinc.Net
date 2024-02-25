public sealed class ProcessTests : TestBase
{
    [Fact]
    public async void Command_Runs()
    {
        var proc = Process.Create("git", "-v");
        var res = await proc.Run();
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().Contain("git version");
    }

    [Fact]
    public async void Command_Listen_MiniZinc_Version()
    {
        using var proc = Process.Create("minizinc", "--version");
        await foreach (var msg in proc.Listen())
        {
            Write("{0}", msg.Content);
        }
    }

    [Fact]
    public async void Command_Run_MiniZinc_Version()
    {
        using var proc = Process.Create("minizinc", "--version");
        var res = await proc.Run();
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().StartWith("MiniZinc to FlatZinc converter, version");
    }

    [Fact]
    public async void Ping_And_Listen()
    {
        var cmd = Command.Create("ping 127.0.0.1 -n 4");
        using var proc = new Process(cmd);
        await foreach (var msg in proc.Listen())
        {
            Write("[{0}|{1}] {2}", cmd.Exe, msg.EventType, msg.Content);
        }
    }

    [Fact]
    public async void Solve_NQueens_With_Timeout()
    {
        var model = """
            int: n = 15;
            array [1..n] of var 1..n: q; % queen in column i is in row q[i]
            
            include "alldifferent.mzn";
            constraint alldifferent(q);                       % distinct rows
            constraint alldifferent([ q[i] + i | i in 1..n]); % distinct diagonals
            constraint alldifferent([ q[i] - i | i in 1..n]); % upwards+downwards

            % search
            solve :: int_search(q, first_fail, indomain_min)
            satisfy;
            """;
        var tmp = Path.GetTempPath().ToDirectory().JoinFile("nqueens.mzn");
        File.WriteAllText(tmp.FullName, model);
        var cmd = Command.Create("minizinc", "-a", "--json-stream", tmp.FullName);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        using var proc = new Process(cmd);
        await foreach (var msg in proc.Listen(cts.Token))
        {
            Write("{0}", msg.EventType);
            if (msg.Content is { } data)
                Write(data);
        }

        var a = 2;
        proc.State.Should().Be(ProcessState.Cancelled);
    }

    public ProcessTests(ITestOutputHelper output)
        : base(output) { }
}
