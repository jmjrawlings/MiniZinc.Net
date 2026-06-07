using MiniZinc.Command;

/// <summary>
/// Unit tests for the structured argument model (<see cref="Arg"/> / <see cref="Args"/>).
/// These are pure — no process is launched.
/// </summary>
public class CommandTests
{
    // ----- Arg -----

    [Fact]
    public void arg_flag_only()
    {
        var arg = new Arg("--all-solutions", null);
        arg.ArgType.ShouldBe(ArgType.FlagOnly);
        arg.Flag.ShouldBe("--all-solutions");
        arg.Value.ShouldBeNull();
        arg.Tokens.ShouldBe(["--all-solutions"]);
        arg.ToString().ShouldBe("--all-solutions");
    }

    [Fact]
    public void arg_value_only()
    {
        var arg = new Arg(null, "model.mzn");
        arg.ArgType.ShouldBe(ArgType.ValueOnly);
        arg.Value.ShouldBe("model.mzn");
        arg.Flag.ShouldBeNull();
        arg.Tokens.ShouldBe(["model.mzn"]);
    }

    [Fact]
    public void arg_option_space_is_two_tokens()
    {
        var arg = new Arg("--solver", "gecode");
        arg.ArgType.ShouldBe(ArgType.FlagOptionSpace);
        arg.Tokens.ShouldBe(["--solver", "gecode"]);
        arg.ToString().ShouldBe("--solver gecode");
    }

    [Fact]
    public void arg_option_equal_is_one_token()
    {
        var arg = new Arg("--solver", "gecode", eq: true);
        arg.ArgType.ShouldBe(ArgType.FlagOptionEqual);
        arg.Tokens.ShouldBe(["--solver=gecode"]);
        arg.ToString().ShouldBe("--solver=gecode");
    }

    [Fact]
    public void arg_requires_flag_or_value()
    {
        Should.Throw<ArgumentException>(() => new Arg(null, null));
    }

    // ----- Args construction -----

    [Fact]
    public void add_tokens_classifies_by_leading_dash()
    {
        var args = new Args();
        args.Add("--flag", "value", "-x");
        args.Tokens.ShouldBe(["--flag", "value", "-x"]);
        var list = args.Values.ToList();
        list[0].ArgType.ShouldBe(ArgType.FlagOnly);
        list[1].ArgType.ShouldBe(ArgType.ValueOnly);
        list[2].ArgType.ShouldBe(ArgType.FlagOnly);
    }

    [Fact]
    public void add_skips_nulls()
    {
        var args = new Args();
        args.Add("a", null, "b");
        args.Tokens.ShouldBe(["a", "b"]);
    }

    [Fact]
    public void add_option_preserves_pairing()
    {
        var args = new Args();
        args.AddOption("--solver", "gecode");
        args.Tokens.ShouldBe(["--solver", "gecode"]);
        args.Values.Single().ArgType.ShouldBe(ArgType.FlagOptionSpace);
    }

    [Fact]
    public void add_value_with_space_is_a_single_token()
    {
        var args = new Args();
        args.AddValue("a path with spaces.mzn");
        args.Tokens.ShouldBe(["a path with spaces.mzn"]);
    }

    [Fact]
    public void add_combines_two_arg_sets()
    {
        var a = new Args();
        a.Add("--one");
        var b = new Args();
        b.Add("--two");
        a.Add(b);
        a.Tokens.ShouldBe(["--one", "--two"]);
    }

    // ----- AddCommandLine (the only place strings are tokenised) -----

    [Fact]
    public void command_line_splits_on_whitespace()
    {
        var args = new Args();
        args.AddCommandLine("--time-limit 1000");
        args.Tokens.ShouldBe(["--time-limit", "1000"]);
    }

    [Fact]
    public void command_line_collapses_extra_whitespace()
    {
        var args = new Args();
        args.AddCommandLine("  -a   -b \t -c ");
        args.Tokens.ShouldBe(["-a", "-b", "-c"]);
    }

    [Fact]
    public void command_line_respects_double_quotes()
    {
        var args = new Args();
        args.AddCommandLine("""--cmdline-data "x = 5" """);
        args.Tokens.ShouldBe(["--cmdline-data", "x = 5"]);
    }

    [Fact]
    public void command_line_empty_is_no_args()
    {
        var args = new Args();
        args.AddCommandLine("   ");
        args.Count.ShouldBe(0);
    }

    // ----- Introspection -----

    [Fact]
    public void has_flag_matches_bare_and_equals_forms()
    {
        var args = new Args();
        args.Add("--statistics");
        args.AddOption("--solver", "chuffed", eq: true);
        args.HasFlag("--statistics").ShouldBeTrue();
        args.HasFlag("--solver").ShouldBeTrue();
        args.HasFlag("--free-search").ShouldBeFalse();
    }

    [Fact]
    public void try_get_option_space_form()
    {
        var args = new Args();
        args.AddCommandLine("--solver gecode");
        args.TryGetOption("--solver", out var value).ShouldBeTrue();
        value.ShouldBe("gecode");
    }

    [Fact]
    public void try_get_option_equals_form()
    {
        var args = new Args();
        args.AddOption("--solver", "chuffed", eq: true);
        args.TryGetOption("--solver", out var value).ShouldBeTrue();
        value.ShouldBe("chuffed");
    }

    [Fact]
    public void try_get_option_absent()
    {
        var args = new Args();
        args.Add("--statistics");
        args.TryGetOption("--solver", out var value).ShouldBeFalse();
        value.ShouldBeNull();
    }

    // ----- Command -----

    [Fact]
    public void command_tokens_round_trip()
    {
        var cmd = Command.From("git").With("remote", "add", "origin", "https://example.com/x.git");
        cmd.Tokens.ShouldBe(["remote", "add", "origin", "https://example.com/x.git"]);
    }

    [Fact]
    public void command_with_methods_produce_tokens_in_order()
    {
        var cmd = Command
            .From("minizinc")
            .WithFlag("--json-stream")
            .WithOption("--solver", "gecode")
            .WithCommandLine("--time-limit 1000")
            .WithValue("model.mzn");

        cmd.Tokens.ShouldBe(
            ["--json-stream", "--solver", "gecode", "--time-limit", "1000", "model.mzn"]
        );
    }

    [Fact]
    public void command_with_does_not_mutate_the_original()
    {
        var basis = Command.From("minizinc").WithFlag("--statistics");
        var derived = basis.WithOption("--solver", "gecode");

        basis.Tokens.ShouldBe(["--statistics"]);
        derived.Tokens.ShouldBe(["--statistics", "--solver", "gecode"]);
    }

    [Fact]
    public void command_forks_from_a_base_are_independent()
    {
        var basis = Command.From("minizinc").WithFlag("--statistics");
        var a = basis.WithFlag("--all-solutions");
        var b = basis.WithOption("--solver", "gecode").WithWorkingDirectory("/tmp");

        basis.Tokens.ShouldBe(["--statistics"]);
        a.Tokens.ShouldBe(["--statistics", "--all-solutions"]);
        b.Tokens.ShouldBe(["--statistics", "--solver", "gecode"]);
        basis.WorkingDirectory.ShouldBeNull();
    }

    [Fact]
    public void command_to_string_includes_exe_and_args()
    {
        var cmd = Command.From("minizinc").WithFlag("--statistics").WithValue("model.mzn");
        cmd.ToString().ShouldBe("minizinc --statistics model.mzn");
    }
}
