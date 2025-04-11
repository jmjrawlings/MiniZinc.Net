using MiniZinc.Command;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

public class CommandTests
{
    [Theory]
    [InlineData("-v")]
    [InlineData("--okay")]
    [InlineData("-a1")]
    public void Parse_Flag_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Value.ShouldBeNull();
        arg.Flag.ShouldNotBeNull();
        arg.ArgType.ShouldBe(ArgType.FlagOnly);
        arg.String.ShouldBe(s);
    }

    [Theory]
    [InlineData("-v 1")]
    [InlineData("--okay \"two\"")]
    [InlineData("-one=2")]
    public void Parse_Flag_And_Value_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Flag.ShouldNotBeNull();
        arg.Value.ShouldNotBeNull();
        arg.String.ShouldBe(s);
    }

    [Fact]
    public void Parse_Url_Flag()
    {
        var url = @"https://github.com/MiniZinc/libminizinc.git";
        var arg = Arg.Parse(url).First();
        arg.Value.ShouldBe(url);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("123")]
    [InlineData("\"asdfasdf asdf\"")]
    public void Parse_Value_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Flag.ShouldBeNull();
        arg.ArgType.ShouldBe(ArgType.ValueOnly);
        arg.Value.ShouldBe(s);
    }

    [Theory]
    [InlineData("--output-json")]
    public void Parse_Value_With_Dashes(string s)
    {
        var args = Arg.Parse(s).ToList();
        args.Count.ShouldBe(1);
        args[0].ArgType.ShouldBe(ArgType.FlagOnly);
        args[0].Flag.ShouldBe(s);
    }
}
