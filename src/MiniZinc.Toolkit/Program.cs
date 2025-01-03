namespace MiniZinc.Toolkit;

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Diagnostics;
using Parser;

class Program
{
    static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("MiniZinc Toolkit");

        var minify = new Command(
            "minify",
            "Minify the given models, removing all comments and extraneous whitespace"
        );
        var format = new Command("format", "Format the given models");

        var filePatternArg = new Argument<List<FileInfo>>(
            name: "files",
            description: "A path or pattern to minizinc files (.mzn, .dzn)",
            parse: result =>
            {
                var cwd = new DirectoryInfo(Directory.GetCurrentDirectory());
                var files = new List<FileInfo>();
                if (result.Tokens.Count == 0)
                {
                    result.ErrorMessage = "Expected a filepath or pattern (eg: ./*.mzn)";
                    return files;
                }

                foreach (var token in result.Tokens)
                {
                    if (File.Exists(token.Value))
                    {
                        files.Add(new FileInfo(token.Value));
                        continue;
                    }

                    foreach (var file in cwd.EnumerateFiles(token.Value))
                    {
                        Console.WriteLine(file.FullName);
                        files.Add(file);
                    }
                }
                return files;
            }
        );

        minify.AddArgument(filePatternArg);
        minify.SetHandler(
            async (context) =>
            {
                var files = context.ParseResult.GetValueForArgument(filePatternArg);
                await Minify(context, files);
            }
        );

        format.AddArgument(filePatternArg);
        format.SetHandler(
            async (context) =>
            {
                var files = context.ParseResult.GetValueForArgument(filePatternArg);
                await Format(context, files);
            }
        );

        root.Add(minify);

        var result = await root.InvokeAsync(args);
        return result;
    }

    static void Minify(InvocationContext context, FileInfo file)
    {
        var console = context.Console;
        if (!Parser.ParseModelFromFile(file, out var model))
            return;

        var minified = model.Write(WriteOptions.Minimal);
        File.WriteAllText(file.FullName, minified);
        console.WriteLine(file.FullName);
    }

    static Task Minify(InvocationContext context, List<FileInfo> files)
    {
        var console = context.Console;
        var watch = Stopwatch.StartNew();

        foreach (var file in files)
            Minify(context, file);

        watch.Stop();
        console.WriteLine($"Minified {files.Count} files in {watch.ElapsedMilliseconds}ms");
        return Task.CompletedTask;
    }

    static void Format(InvocationContext context, FileInfo file)
    {
        var console = context.Console;
        if (!Parser.ParseModelFromFile(file, out var model))
        {
            context.ExitCode = 100;
            return;
        }

        var output = model.Write(WriteOptions.Pretty);
        File.WriteAllText(file.FullName, output);
        console.WriteLine(file.FullName);
    }

    static Task Format(InvocationContext context, List<FileInfo> files)
    {
        var console = context.Console;
        var watch = Stopwatch.StartNew();

        foreach (var file in files)
            Format(context, file);

        watch.Stop();
        console.WriteLine($"Formatted {files.Count} files in {watch.ElapsedMilliseconds}ms");
        return Task.CompletedTask;
    }
}
