namespace MiniZinc.Tests;

public sealed class CommandTests : TestBase
{
    [Theory]
    [InlineData("-v")]
    [InlineData("--okay")]
    [InlineData("-a1")]
    public void Parse_Flag_Only_Arg(string s)
    {
        var arg = Arg.Parse(s);
        Assert.Null(arg.Value);
        Assert.Null(arg.Sep);
    }

    [Theory]
    [InlineData("-v 1")]
    [InlineData("--okay \"two\"")]
    [InlineData("-one=2")]
    public void Parse_Flag_And_Value_Arg(string s)
    {
        var arg = Arg.Parse(s);
        Assert.NotNull(arg.Flag);
        Assert.NotNull(arg.Value);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("123")]
    [InlineData("\"asdfasdf asdf\"")]
    public void Parse_Value_Only_Arg(string s)
    {
        var arg = Arg.Parse(s);
        Assert.Null(arg.Flag);
        Assert.Null(arg.Sep);
        Assert.Equal(arg.Value, s);
    }

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
    public async void Command_Stream_MiniZinc_Version()
    {
        var cmd = Command.Create("minizinc", "--version");
        await foreach (var msg in cmd.Stream(_logger))
        {
            var a = 2;
        }
    }

    [Fact]
    public async void Command_Run_MiniZinc_Version()
    {
        var cmd = Command.Create("minizinc", "--version");
        var res = await cmd.Run();
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().Contain("minizinc");
    }

    public CommandTests(LoggingFixture logging, ITestOutputHelper output)
        : base(logging, output) { }
}
