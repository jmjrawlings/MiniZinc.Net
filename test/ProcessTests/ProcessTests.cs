using MiniZinc.Command;
using MiniZinc.Core;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public class ProcessTests
{
    [Test]
    public async Task Command_Runs()
    {
        var cmd = new Command("minizinc", "--version");
        var result = await cmd.Run();
        await Assert.That(result.Status).IsEqualTo(ProcessStatus.Ok);
    }

    [Test]
    public async Task Command_Listens()
    {
        var cmd = new Command("minizinc", "--version");
        string output = "";
        await foreach (var msg in cmd.Watch())
        {
            output = msg.Content;
        }

        await Assert.That(output).IsNotEmpty();
    }

    [Test]
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
        var proc = new CommandRunner(cmd);
        ProcessMessage result = default;
        await foreach (var msg in proc.Watch(cts.Token))
        {
            Console.WriteLine(msg.EventType);
            result = msg;
            if (msg.Content is { } data)
                Console.WriteLine(data);
        }
    }
}
