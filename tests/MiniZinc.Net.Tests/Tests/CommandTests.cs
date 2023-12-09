namespace MiniZinc.Tests;

public sealed class CommandTests
{
    [Fact]
    public void Command_Runs_Sync()
    {
        var cmd = Command.Create("git", "-v");
        var res = cmd.Run().Result;
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().Contain("git version");
    }

    [Fact]
    public async void Command_Runs_Async()
    {
        var cmd = Command.Create("git", "-v");
        var res = await cmd.Run();
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().Contain("git version");
    }

    [Fact]
    public async void Command_Stream()
    {
        var cmd = Command.Create("minizinc", "-v");
        await foreach (var msg in cmd.Stream())
        {
            var a = 2;
        }
    }
}
