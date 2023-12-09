namespace MiniZinc.Tests;

public sealed class CommandTests
{
    [Fact]
    public void Command_Runs_Sync()
    {
        var cmd = Command.Create("git", "-v");
        var res = cmd.RunSync();
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
}
