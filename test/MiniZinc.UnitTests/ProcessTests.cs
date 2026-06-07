using MiniZinc.Command;
using MiniZinc.Core;
using static System.Console;

public class ProcessTests
{
    [Fact]
    public async Task test_command_runs()
    {
        var cmd = Command.From("minizinc").With("--version");
        var result = await cmd.RunAsync(Cancellation);
        result.Status.ShouldBe(ProcessStatus.Ok);
    }

    [Fact]
    public async Task test_command_watch()
    {
        var cmd = Command.From("minizinc").With("--version");
        string? output = null;
        await foreach (var msg in cmd.WatchAsync(Cancellation))
            if (msg is ProcessStdOut o)
            {
                output = o.Text;
                break;
            }
        WriteLine(output);
        output.ShouldNotBeNullOrEmpty();
    }

    [Fact]
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
        await File.WriteAllTextAsync(tmp.FullName, model, Cancellation);
        var cmd = Command
            .From("minizinc")
            .With(
            "--solver",
            "Gecode",
            "--all-solutions",
            "--json-stream",
            tmp.FullName
        );
        var timeout = TimeSpan.FromSeconds(1);
        var cts = new CancellationTokenSource(timeout);
        await foreach (var msg in cmd.WatchAsync(cts.Token))
        {
            WriteLine(msg);
            WriteLine("------------------------------------");
        }
    }
}
