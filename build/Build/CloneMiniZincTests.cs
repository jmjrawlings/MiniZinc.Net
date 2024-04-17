namespace Build;

using CommunityToolkit.Diagnostics;
using MiniZinc.Build;
using MiniZinc.Process;

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

        async Task Run(params string[] args)
        {
            var cmd = Command.Create(args).WithWorkingDirectory(cloneDir);
            Console.WriteLine(cmd.String);
            var result = await cmd.Run();
            Guard.IsEqualTo((int)result.Status, (int)ProcessStatus.Ok);
        }

        await Run("git", "init");
        await Run("git", "remote", "add", "origin", url);
        await Run("git", "sparse-checkout", "set", "tests/spec");
        await Run("git", "fetch", "origin", "master");
        await Run("git", "checkout", "master");

        var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
        var targetDir = Repo.LibMiniZincDir.CreateOrClear();
        Console.WriteLine($"Copying tests to {targetDir}");
        sourceDir.CopyContentsTo(targetDir);
    }
}
