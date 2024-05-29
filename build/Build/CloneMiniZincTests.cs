namespace Build;

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

        async Task Exe(params string[] args)
        {
            var cmd = new Command("git", args);
            Console.WriteLine(cmd.String);
            var result = await cmd.Run(workingDir: cloneDir.FullName);
            Guard.IsEqualTo((int)result.Status, (int)ProcessStatus.Ok);
        }

        await Exe("git", "init");
        await Exe("git", "remote", "add", "origin", url);
        await Exe("git", "sparse-checkout", "set", "tests/spec");
        await Exe("git", "fetch", "origin", "master");
        await Exe("git", "checkout", "master");

        var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
        var targetDir = Repo.LibMiniZincDir.CreateOrClear();
        Console.WriteLine($"Copying tests to {targetDir}");
        sourceDir.CopyContentsTo(targetDir);
    }
}
