namespace MiniZinc.Tests.Yaml;

using MiniZinc.Build;
using MiniZinc.TestSpec;
using MiniZinc.TestSpec.Model;
using MiniZinc.TestSpec.Yaml;

/// <summary>
/// Validates the new YAML scanner + interpreter over every .mzn file in the
/// spec tree. Reports parse errors with file:line:col, counts test cases by
/// kind, and surfaces SkipReasons by frequency.
/// </summary>
public static class YamlSmokeTest
{
    public sealed record Stats(
        int FilesScanned,
        int FilesWithPreamble,
        int DocumentsParsed,
        int FilesWithScannerErrors,
        int TestCasesInterpreted,
        int Suites,
        IReadOnlyDictionary<TestKind, int> CasesByKind,
        IReadOnlyList<string> ScannerErrors,
        IReadOnlyDictionary<string, int> SkipReasons
    );

    public static Stats Run(DirectoryInfo specRoot)
    {
        int scanned = 0;
        int withPreamble = 0;
        int docs = 0;
        int scannerErrors = 0;
        int casesInterpreted = 0;
        int suiteCount = 0;
        var scannerErrorList = new List<string>();
        var byKind = new Dictionary<TestKind, int>();
        var skipReasons = new Dictionary<string, int>();
        bool trace = Environment.GetEnvironmentVariable("YAML_TRACE") == "1";

        var suitesFile = specRoot.JoinFile("suites.yml");
        IReadOnlyList<TestSuite> suites = Array.Empty<TestSuite>();
        if (suitesFile.Exists)
        {
            try
            {
                suites = YamlSpecParser.ParseSuites(suitesFile);
                suiteCount = suites.Count;
                docs += 1;
            }
            catch (YamlParseException ex)
            {
                scannerErrors++;
                scannerErrorList.Add(ex.Message);
            }
        }

        foreach (var mzn in specRoot.EnumerateFiles("*.mzn", SearchOption.AllDirectories))
        {
            scanned++;
            string source = File.ReadAllText(mzn.FullName);
            string? raw = TestCommentExtractor.ExtractRawYaml(source);
            if (raw is null)
                continue;
            withPreamble++;

            string rel = Path.GetRelativePath(specRoot.FullName, mzn.FullName).Replace('\\', '/');
            if (trace)
            {
                Console.Write($"... {rel}\n");
                Console.Out.Flush();
            }
            // Scanner pass (collects raw errors separately from interpreter)
            try
            {
                var scanner = new YamlScanner(raw, rel);
                var documents = scanner.ParseStream();
                docs += documents.Count;
            }
            catch (YamlParseException ex)
            {
                scannerErrors++;
                scannerErrorList.Add(ex.Message);
            }

            // Interpreter pass (full Suites/TestCase build)
            var cases = YamlSpecParser.ParseTestCases(mzn, specRoot, "default", null);
            if (cases is null)
                continue;
            foreach (var c in cases)
            {
                casesInterpreted++;
                byKind[c.Kind] = byKind.TryGetValue(c.Kind, out int n) ? n + 1 : 1;
                if (c.SkipReason is not null)
                {
                    skipReasons[c.SkipReason] = skipReasons.TryGetValue(c.SkipReason, out int m) ? m + 1 : 1;
                    if (Environment.GetEnvironmentVariable("YAML_SHOW_SKIPS") == "1")
                        Console.Error.WriteLine($"SKIP: {rel}: {c.SkipReason}");
                }
            }
        }

        return new Stats(
            scanned,
            withPreamble,
            docs,
            scannerErrors,
            casesInterpreted,
            suiteCount,
            byKind,
            scannerErrorList,
            skipReasons);
    }
}
