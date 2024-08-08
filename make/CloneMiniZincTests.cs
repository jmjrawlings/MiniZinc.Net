namespace Make;

using CommunityToolkit.Diagnostics;
using MiniZinc.Build;
using MiniZinc.Command;

public static class CloneLibMiniZincTests
{
    public static async Task Run()
    {
        Console.WriteLine("Cloning libminiznc tests");
        var url = $"https://github.com/MiniZinc/libminizinc.git";
        var cloneDir = Environment
            .CurrentDirectory.ToDirectory()
            .JoinDir("libminiznc")
            .CreateOrClear();

        async Task Git(params string[] args)
        {
            var cmd = new Command("git", args).WithWorkingDirectory(cloneDir);
            Console.WriteLine(cmd);
            var result = await cmd.Run();
            Guard.IsEqualTo((int)result.Status, (int)ProcessStatus.Ok);
        }

        await Git("init");
        await Git("remote", "add", "origin", url);
        await Git("sparse-checkout", "set", "tests/spec");
        await Git("fetch", "origin", "master");
        await Git("checkout", "master");

        var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
        var targetDir = Repo.LibMiniZincDir.CreateOrClear();
        Console.WriteLine($"Copying tests to {targetDir}");
        sourceDir.CopyContentsTo(targetDir);
    }
}
