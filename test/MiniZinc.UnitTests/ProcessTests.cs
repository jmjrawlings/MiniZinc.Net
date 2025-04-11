using MiniZinc.Command;
using MiniZinc.Core;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

public class ProcessTests
{
    private readonly ITestOutputHelper _out;

    public ProcessTests(ITestOutputHelper @out)
    {
        _out = @out;
    }

    [Fact]
    public async Task Command_Runs()
    {
        var cmd = new Command("minizinc", "--version");
        var result = await cmd.Run();
        result.Status.ShouldBe(ProcessStatus.Ok);
    }

    [Fact]
    public async Task Command_Listens()
    {
        var cmd = new Command("minizinc", "--version");
        string? output = null;
        await foreach (var msg in cmd.Watch())
            output ??= msg.Content;

        _out.WriteLine(output);
        output.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task Solve_NQueens_With_Timeout()
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
        await File.WriteAllTextAsync(tmp.FullName, model);
        var cmd = new Command("minizinc", "-a", "--json-stream", tmp.FullName);
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        ProcessMessage result = default;
        await foreach (var msg in cmd.Watch(cts.Token))
        {
            _out.WriteLine(msg.EventType.ToString());
            result = msg;
            if (msg.Content is { } data)
                _out.WriteLine(data);
        }
    }
}
