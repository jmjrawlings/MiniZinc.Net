using MiniZinc.Command;
using MiniZinc.Core;
using static System.Console;

public class ProcessTests
{
    [Test]
    public async Task test_command_runs()
    {
        var cmd = new Command("minizinc", "--version");
        var result = await cmd.Run();
        result.Status.ShouldBe(ProcessStatus.Ok);
    }

    [Test]
    public async Task test_command_listens()
    {
        var cmd = new Command("minizinc", "--version");
        string? output = null;
        await foreach (var msg in cmd.Watch())
            output ??= msg.Content;
        WriteLine(output);
        output.ShouldNotBeEmpty();
    }

    [Test]
    public async Task test_solve_nqueens_with_timeout()
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
            WriteLine(msg.EventType.ToString());
            result = msg;
            if (msg.Content is { } data)
                WriteLine(data);
        }
    }
}
