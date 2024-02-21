public sealed class CommandTests : TestBase
{
    [Theory]
    [InlineData("-v")]
    [InlineData("--okay")]
    [InlineData("-a1")]
    public void Parse_Flag_Only_Arg(string s)
    {
        var arg = CommandArg.Parse(s) ?? default;
        Assert.Null(arg.Value);
        Assert.False(arg.Eq);
        Assert.Equal(arg.String, s);
    }

    [Theory]
    [InlineData("-v 1")]
    [InlineData("--okay \"two\"")]
    [InlineData("-one=2")]
    public void Parse_Flag_And_Value_Arg(string s)
    {
        var arg = CommandArg.Parse(s) ?? default;
        Assert.NotNull(arg.Flag);
        Assert.NotNull(arg.Value);
        Assert.Equal(arg.String, s);
    }

    [Fact]
    public void Parse_Url_Flag()
    {
        var url = @"https://github.com/MiniZinc/libminizinc.git";
        var arg = CommandArg.Parse(url) ?? default;
        Assert.Equal(arg.Value, url);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("123")]
    [InlineData("\"asdfasdf asdf\"")]
    public void Parse_Value_Only_Arg(string s)
    {
        var arg = CommandArg.Parse(s) ?? default;
        Assert.Null(arg.Flag);
        Assert.False(arg.Eq);
        Assert.Equal(arg.Value, s);
    }

    [Fact]
    public async void Command_Runs()
    {
        var cmd = Command.Create("git", "-v");
        var res = await cmd.Run();
        res.StdErr.Should().BeEmpty();
        res.StdOut.Should().Contain("git version");
    }

    [Fact]
    public async void Command_Listen_MiniZinc_Version()
    {
        var cmd = Command.Create("minizinc", "--version");
        using var process = cmd.ToProcess();
        await foreach (var msg in process)
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
        res.StdOut.Should().StartWith("MiniZinc to FlatZinc converter, version");
    }

    [Fact]
    public async void Ping_And_Listen()
    {
        var cmd = Command.Create("ping 127.0.0.1 -n 4");
        using var proc = cmd.ToProcess();
        await foreach (var msg in proc)
        {
            Write("[{0}|{1}] {2}", cmd.Exe, msg.MessageType, msg.Content);
        }
    }

    public CommandTests(ITestOutputHelper output)
        : base(output) { }
}
