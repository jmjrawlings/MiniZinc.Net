namespace Build;

using MiniZinc.Build;
using MiniZinc.Process;

public static class CloneLibMiniZincTests
{
    public static async Task Run()
    {
        var url = $"https://github.com/MiniZinc/libminizinc.git";
        var libDir = Repo.LibMiniZincDir.CreateOrClear();
        var cloneDir = Environment
            .CurrentDirectory.JoinPath(Path.GetFileNameWithoutExtension(Path.GetTempFileName()))
            .ToDirectory()
            .CreateOrClear();

        async Task<ProcessResult> Run(params string[] args)
        {
            var cmd = Command.Create(args).WithWorkingDirectory(cloneDir);
            var result = await cmd.Run();
            result.EnsureSuccess();
            return result;
        }

        await Run("git", "init");
        await Run("git", "remote", "add", "origin", url);
        await Run("git", "sparse-checkout", "set", "tests/spec");
        await Run("git", "fetch", "origin", "master");
        await Run("git", "checkout", "master");

        var sourceDir = cloneDir.JoinDir("tests", "spec").EnsureExists();
        var targetDir = libDir;
        sourceDir.CopyContentsTo(targetDir);
        cloneDir.Delete(true);
    }
}
