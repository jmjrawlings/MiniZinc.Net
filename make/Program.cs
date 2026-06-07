using System.CommandLine;
using CommunityToolkit.Diagnostics;
using Make;
using MiniZinc.Build;
using MiniZinc.Command;
using MiniZinc.TestSpec;
using MiniZinc.TestSpec.Model;
using MiniZinc.Tests.Yaml;
using static Make.Prelude;
using Cmd = MiniZinc.Command.Command;
using Command = System.CommandLine.Command;

var root = new RootCommand("MiniZinc.NET build options");

// Add(
//     "--make-parser-tests",
//     "Generate parser tests",
//     async () =>
//     {
//         var spec = LoadSpec();
//         var builder = new ParserTestBuilder();
//         var source = builder.Build(spec);
//         var file = Repo
//             .TestDir.JoinDir("MiniZinc.IntegrationTests")
//             .JoinFile($"{builder.ClassName}.cs");
//         await File.WriteAllTextAsync(file.FullName, source);
//     }
// );
//
Add(
    "--make-client-tests",
    "Generate client tests",
    () =>
    {
        var specDir = Repo.TestSpecDir;
        var suitesFile = Repo.TestSpecYaml;
        Console.WriteLine($"Parsing {suitesFile.FullName}");
        var suites = YamlSpecParser.ParseSuites(suitesFile);

        var skipFile = specDir.JoinFile("skip-list.yml");
        var skipList = YamlSkipList.LoadOrEmpty(skipFile);
        if (skipFile.Exists)
            Console.WriteLine($"Loaded skip-list {skipFile.FullName}");

        foreach (var suite in suites)
        {
            // Each suite's include globs are matched against the spec tree; a
            // single .mzn file can belong to several suites (with that suite's
            // solver/option defaults applied).
            var cases = new List<TestCase>();
            foreach (var glob in suite.IncludeGlobs)
            {
                foreach (var mzn in specDir.EnumerateFiles(glob, SearchOption.AllDirectories))
                {
                    if (mzn.Extension != ".mzn")
                        continue;
                    var parsed = YamlSpecParser.ParseTestCases(mzn, specDir, suite.Name, suite);
                    if (parsed is not null)
                        cases.AddRange(parsed);
                }
            }

            var testClass = $"IntegrationTests{suite.Name.ToClassName()}";
            var testFile = Repo.IntegrationTestsDir.JoinFile($"{testClass}.cs");
            var testSource = ClientTestsBuilder.Build(cases, testClass, skipList);
            File.WriteAllText(testFile.FullName, testSource);
            Console.WriteLine($"  {suite.Name} -> {testClass}.cs ({cases.Count} cases)");
        }

        return Task.CompletedTask;
    }
);

Add(
    "--sync-libminizinc-tests",
    "Sync the upstream libminizinc test spec at the pinned commit into spec/",
    async () =>
    {
        var force = Environment.GetCommandLineArgs().Contains("--force");

        var targetDir = Repo.TestSpecDir;
        if (!targetDir.Exists)
            targetDir.Create();

        // Refuse to sync if the target dir has uncommitted changes (unless --force).
        var statusCmd = Cmd.From("git")
            .With("status", "--porcelain", "--", targetDir.FullName)
            .WithWorkingDirectory(Repo.SolutionDir.FullName);
        var statusResult = await statusCmd.RunAsync();
        if (statusResult.Status == ProcessStatus.Ok && !string.IsNullOrWhiteSpace(statusResult.StdOut) && !force)
        {
            Console.Error.WriteLine("Refusing to sync: uncommitted changes under spec/:");
            Console.Error.WriteLine(statusResult.StdOut);
            Console.Error.WriteLine("Re-run with --force to override.");
            Environment.Exit(1);
        }

        var cacheDir = Repo.LibMiniZincCacheDir;
        if (!cacheDir.Exists)
        {
            Console.WriteLine($"Cloning libminizinc@{Repo.LibMiniZincCommit[..12]} to {cacheDir.FullName}");
            cacheDir.Create();

            async Task Git(params string[] gitArgs)
            {
                var cmd = Cmd.From("git").With(gitArgs).WithWorkingDirectory(cacheDir.FullName);
                Console.WriteLine($"  $ git {string.Join(' ', gitArgs)}");
                var result = await cmd.RunAsync();
                Guard.IsEqualTo((int)result.Status, (int)ProcessStatus.Ok);
            }

            await Git("init");
            await Git("remote", "add", "origin", "https://github.com/MiniZinc/libminizinc.git");
            await Git("sparse-checkout", "set", "tests/spec");
            await Git("fetch", "--depth", "1", "origin", Repo.LibMiniZincCommit);
            await Git("checkout", "FETCH_HEAD");
        }
        else
        {
            Console.WriteLine($"Using cached libminizinc tree at {cacheDir.FullName}");
        }

        var sourceDir = cacheDir.JoinDir("tests", "spec");
        if (!sourceDir.Exists)
        {
            Console.Error.WriteLine($"Cache missing tests/spec subtree at {sourceDir.FullName}");
            Environment.Exit(1);
        }

        // Preserve list — keep these in the target across syncs.
        var preserve = new HashSet<string>(StringComparer.Ordinal) { "suites.yml", "skip-list.yml" };

        // Wipe everything in target except preserved files.
        int filesRemoved = 0;
        int dirsRemoved = 0;
        foreach (var f in targetDir.EnumerateFiles())
        {
            if (preserve.Contains(f.Name))
                continue;
            f.Delete();
            filesRemoved++;
        }
        foreach (var d in targetDir.EnumerateDirectories())
        {
            d.Delete(recursive: true);
            dirsRemoved++;
        }

        // Copy upstream tree in, skipping anything that would clash with preserved files.
        int filesCopied = CopyTree(sourceDir, targetDir, preserve);

        Console.WriteLine($"Sync complete: copied {filesCopied} files, removed {filesRemoved} files + {dirsRemoved} dirs.");
        Console.WriteLine("Review the diff with `git status test/MiniZinc.IntegrationTests/spec/`.");

        static int CopyTree(DirectoryInfo src, DirectoryInfo dst, HashSet<string> preserve)
        {
            int count = 0;
            foreach (var f in src.EnumerateFiles())
            {
                if (preserve.Contains(f.Name))
                    continue;
                f.CopyTo(dst.JoinFile(f.Name).FullName, overwrite: true);
                count++;
            }
            foreach (var d in src.EnumerateDirectories())
            {
                var dstSub = dst.JoinDir(d.Name);
                if (!dstSub.Exists)
                    dstSub.Create();
                count += CopyTree(d, dstSub, preserve);
            }
            return count;
        }
    }
);

Add(
    "--smoke-test-yaml",
    "Run the new YAML scanner + interpreter over every spec file and report errors",
    () =>
    {
        var stats = YamlSmokeTest.Run(Repo.TestSpecDir);
        Console.WriteLine($"Suites: {stats.Suites}");
        Console.WriteLine($"Scanned {stats.FilesScanned} .mzn files");
        Console.WriteLine($"  with preamble: {stats.FilesWithPreamble}");
        Console.WriteLine($"  documents parsed: {stats.DocumentsParsed}");
        Console.WriteLine($"  scanner errors: {stats.FilesWithScannerErrors}");
        Console.WriteLine($"  test cases interpreted: {stats.TestCasesInterpreted}");
        Console.WriteLine();
        Console.WriteLine("By kind:");
        foreach (var kv in stats.CasesByKind.OrderByDescending(kv => kv.Value))
            Console.WriteLine($"  {kv.Key,-14} {kv.Value}");
        if (stats.ScannerErrors.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Scanner errors:");
            foreach (var err in stats.ScannerErrors.Take(20))
                Console.WriteLine($"  {err}");
            if (stats.ScannerErrors.Count > 20)
                Console.WriteLine($"  ... and {stats.ScannerErrors.Count - 20} more");
        }
        if (stats.SkipReasons.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine($"Skip reasons ({stats.SkipReasons.Sum(kv => kv.Value)} cases):");
            foreach (var kv in stats.SkipReasons.OrderByDescending(kv => kv.Value).Take(20))
                Console.WriteLine($"  {kv.Value,4}x  {kv.Key}");
        }
        return Task.CompletedTask;
    }
);

var result = await root.InvokeAsync(args);
return result;
void Add(string name, string desc, Func<Task> handler)
{
    var command = new Command(name: name, description: desc);
    command.SetHandler(handler);
    root.AddCommand(command);
}
