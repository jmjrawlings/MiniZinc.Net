using MiniZinc.Command;

public class CommandTests
{
    [Test]
    [Arguments("-v")]
    [Arguments("--okay")]
    [Arguments("-a1")]
    public void test_parse_arg_flag_only(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Value.ShouldBeNull();
        arg.Flag.ShouldNotBeNull();
        arg.ArgType.ShouldBe(ArgType.FlagOnly);
        arg.String.ShouldBe(s);
    }

    [Test]
    [Arguments("-v 1")]
    [Arguments("--okay \"two\"")]
    [Arguments("-one=2")]
    public void test_parse_arg_flag_and_value(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Flag.ShouldNotBeNull();
        arg.Value.ShouldNotBeNull();
        arg.String.ShouldBe(s);
    }

    [Test]
    public void test_parse_arg_url_flag()
    {
        var url = @"https://github.com/MiniZinc/libminizinc.git";
        var arg = Arg.Parse(url).First();
        arg.Value.ShouldBe(url);
    }

    [Test]
    [Arguments("x")]
    [Arguments("123")]
    [Arguments("\"asdfasdf asdf\"")]
    public void test_parse_arg_value_only(string s)
    {
        var arg = Arg.Parse(s).First();
        arg.Flag.ShouldBeNull();
        arg.ArgType.ShouldBe(ArgType.ValueOnly);
        arg.Value.ShouldBe(s);
    }

    [Test]
    [Arguments("--output-json")]
    public void test_parse_arg_value_with_dashes(string s)
    {
        var args = Arg.Parse(s).ToList();
        args.Count.ShouldBe(1);
        args[0].ArgType.ShouldBe(ArgType.FlagOnly);
        args[0].Flag.ShouldBe(s);
    }
}
