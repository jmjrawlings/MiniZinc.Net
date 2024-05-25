public sealed class ProcessTests : TestBase
{
    [Fact]
    public async void Command_Runs()
    {
        var cmd = Command.Create("minizinc", "--version");
        var proc = await cmd.Run();
        proc.Status.Should().Be(ProcessStatus.Ok);
    }

    [Fact]
    public async void Command_Listens()
    {
        var cmd = Command.Create("minizinc", "--version");
        var proc = new Process(cmd);
        await foreach (var msg in proc.Watch())
        {
            Write("{0}", msg.Content);
        }

        proc.Status.Should().Be(ProcessStatus.Ok);
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
        var proc = new Process(cmd);
        await foreach (var msg in proc.Watch(cts.Token))
        {
            Write("{0}", msg.EventType);
            if (msg.Content is { } data)
                Write(data);
        }

        proc.Status.Should().Be(ProcessStatus.Cancelled);
    }

    public ProcessTests(ITestOutputHelper output)
        : base(output) { }
}
