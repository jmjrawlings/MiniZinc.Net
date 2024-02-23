public sealed class CommandTests : TestBase
{
    [Theory]
    [InlineData("-v")]
    [InlineData("--okay")]
    [InlineData("-a1")]
    public void Parse_Flag_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Value.Should().BeNull();
        arg.ArgType.Should().Be(ArgType.FlagOnly);
        arg.String.Should().Be(s);
    }

    [Theory]
    [InlineData("-v 1")]
    [InlineData("--okay \"two\"")]
    [InlineData("-one=2")]
    public void Parse_Flag_And_Value_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        Assert.NotNull(arg.Flag);
        Assert.NotNull(arg.Value);
        Assert.Equal(arg.String, s);
    }

    [Fact]
    public void Parse_Url_Flag()
    {
        var url = @"https://github.com/MiniZinc/libminizinc.git";
        var arg = Arg.Parse(url).First();
        Assert.Equal(arg.Value, url);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("123")]
    [InlineData("\"asdfasdf asdf\"")]
    public void Parse_Value_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Flag.Should().BeNull();
        arg.ArgType.Should().Be(ArgType.ValueOnly);
        arg.Value.Should().Be(s);
    }

    [Theory]
    [InlineData("--output-json")]
    public void Parse_Value_With_Dashes(string s)
    {
        var arg = Arg.ParseSingle(s);
        arg.ArgType.Should().Be(ArgType.FlagOnly);
        arg.Flag.Should().Be(s);
    }

    public CommandTests(ITestOutputHelper output)
        : base(output) { }
}
