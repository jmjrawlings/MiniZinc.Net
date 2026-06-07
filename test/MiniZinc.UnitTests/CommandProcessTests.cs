using MiniZinc.Command;

/// <summary>
/// Behavioural tests for the process runner. These drive a real child process
/// via <c>sh</c> (so they need no MiniZinc install) and assert the contract the
/// solver relies on: complete output, correct status, space-safe arguments,
/// and prompt cancellation.
/// </summary>
public class CommandProcessTests
{
    [Fact]
    public async Task run_captures_all_stdout_including_final_unterminated_line()
    {
        // "last" has no trailing newline and lands right before exit — exactly the
        // output the old complete-on-exit race used to drop.
        var cmd = Command.From("sh").With("-c", "printf 'line1\\nline2\\nlast'");
        var result = await cmd.RunAsync(Cancellation);
        result.Status.ShouldBe(ProcessStatus.Ok);
        result.ExitCode.ShouldBe(0);
        result.IsOk.ShouldBeTrue();
        result.StdOut.ShouldContain("line1");
        result.StdOut.ShouldContain("line2");
        result.StdOut.ShouldContain("last");
    }

    [Fact]
    public async Task run_reports_nonzero_exit_as_error()
    {
        var cmd = Command.From("sh").With("-c", "exit 7");
        var result = await cmd.RunAsync(Cancellation);
        result.ExitCode.ShouldBe(7);
        result.Status.ShouldBe(ProcessStatus.Error);
        result.IsError.ShouldBeTrue();
    }

    [Fact]
    public async Task run_captures_stderr()
    {
        var cmd = Command.From("sh").With("-c", "printf boom 1>&2");
        var result = await cmd.RunAsync(Cancellation);
        result.StdErr.ShouldContain("boom");
    }

    [Fact]
    public async Task run_captures_both_streams()
    {
        var cmd = Command.From("sh").With("-c", "printf out; printf oops 1>&2");
        var result = await cmd.RunAsync(Cancellation);
        result.StdOut.ShouldContain("out");
        result.StdErr.ShouldContain("oops");
    }

    [Fact]
    public async Task argument_with_spaces_is_passed_as_one_token()
    {
        // $@ expands the trailing operands; printf repeats the format once each.
        // If "a b" were split, we'd see "[a][b]" instead of "[a b]".
        var cmd = Command.From("sh").With("-c", "printf '[%s]' \"$@\"", "sh", "a b", "c");
        var result = await cmd.RunAsync(Cancellation);
        result.StdOut.ShouldContain("[a b]");
        result.StdOut.ShouldContain("[c]");
        result.StdOut.ShouldNotContain("[a]");
    }

    [Fact]
    public async Task watch_streams_lines_then_exit()
    {
        var cmd = Command.From("sh").With("-c", "printf 'a\\nb\\n'");
        var stdout = new List<string>();
        bool exited = false;
        await foreach (var msg in cmd.WatchAsync(Cancellation))
        {
            switch (msg)
            {
                case ProcessStdOut o:
                    stdout.Add(o.Text);
                    break;
                case ProcessExited:
                    exited = true;
                    break;
            }
        }
        stdout.ShouldBe(["a", "b"]);
        exited.ShouldBeTrue();
    }

    [Fact]
    public async Task cancellation_kills_the_process_promptly()
    {
        var cmd = Command.From("sh").With("-c", "sleep 30");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));
        var result = await cmd.RunAsync(cancellation: cts.Token);
        result.Status.ShouldBe(ProcessStatus.Cancelled);
        result.Duration.ShouldBeLessThan(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task missing_executable_surfaces_an_error()
    {
        var cmd = Command.From("minizinc-net-no-such-binary-xyz");
        await Should.ThrowAsync<Exception>(async () => await cmd.RunAsync());
    }
}
