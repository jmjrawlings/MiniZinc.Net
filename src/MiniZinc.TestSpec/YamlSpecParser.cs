namespace MiniZinc.TestSpec;

using System.Globalization;
using MiniZinc.TestSpec.Model;
using MiniZinc.TestSpec.Yaml;

/// <summary>
/// Interprets a parsed YAML tree (from <see cref="YamlScanner"/>) as
/// libminizinc test specs and test cases.
/// </summary>
public static class YamlSpecParser
{
    /// <summary>
    /// Parse the top-level suites.yml file into a list of TestSuites.
    /// Top-level shape is a mapping of suite-name → !Suite mapping.
    /// </summary>
    public static IReadOnlyList<TestSuite> ParseSuites(FileInfo suitesFile)
    {
        string text = File.ReadAllText(suitesFile.FullName);
        var scanner = new YamlScanner(text, suitesFile.Name);
        var docs = scanner.ParseStream();
        if (docs.Count == 0)
            return Array.Empty<TestSuite>();
        if (docs[0] is not YamlMapping root)
            throw new YamlParseException("expected top-level mapping in suites.yml", 1, 1, suitesFile.Name);

        var suites = new List<TestSuite>();
        foreach (var entry in root.Entries)
        {
            if (entry.Value is not YamlMapping suiteMap)
                throw new YamlParseException($"suite '{entry.Key.Value}' must be a mapping",
                    entry.Value.Line, entry.Value.Column, suitesFile.Name);
            suites.Add(ParseSuite(entry.Key.Value, suiteMap));
        }
        return suites;
    }

    private static TestSuite ParseSuite(string name, YamlMapping map)
    {
        IReadOnlyList<string>? includes = null;
        IReadOnlyList<string>? solvers = null;
        IReadOnlyDictionary<string, string?>? options = null;
        bool strict = true;

        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "includes":
                    includes = ExpectStringList(entry.Value);
                    break;
                case "solvers":
                    solvers = ExpectStringList(entry.Value);
                    break;
                case "options":
                    options = ExpectStringDict(entry.Value);
                    break;
                case "strict":
                    strict = ExpectBool(entry.Value);
                    break;
            }
        }

        return new TestSuite(
            name,
            includes ?? Array.Empty<string>(),
            solvers,
            options,
            strict);
    }

    /// <summary>
    /// Parse all test case documents from a single .mzn file's YAML preamble.
    /// Returns null if the file has no preamble; otherwise one TestCase per document.
    /// SkipReason is populated when the parser hits an unsupported shape.
    /// </summary>
    public static IReadOnlyList<TestCase>? ParseTestCases(
        FileInfo mznFile,
        DirectoryInfo specRoot,
        string suiteName,
        TestSuite? suiteDefaults = null)
    {
        string source = File.ReadAllText(mznFile.FullName);
        string? raw = TestCommentExtractor.ExtractRawYaml(source);
        if (raw is null)
            return null;

        string rel = Path.GetRelativePath(specRoot.FullName, mznFile.FullName).Replace('\\', '/');
        IReadOnlyList<YamlNode> docs;
        try
        {
            var scanner = new YamlScanner(raw, rel);
            docs = scanner.ParseStream();
        }
        catch (YamlParseException ex)
        {
            return new[]
            {
                MakeSkip(rel, suiteName, $"yaml parse error: {ex.Message}"),
            };
        }

        var cases = new List<TestCase>();
        foreach (var doc in docs)
        {
            try
            {
                cases.Add(InterpretTestCase(doc, rel, suiteName, suiteDefaults));
            }
            catch (YamlParseException ex)
            {
                cases.Add(MakeSkip(rel, suiteName, $"yaml parse error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                cases.Add(MakeSkip(rel, suiteName, $"{ex.GetType().Name}: {ex.Message}"));
            }
        }
        return cases;
    }

    private static TestCase MakeSkip(string path, string suite, string reason) =>
        new()
        {
            Suite = suite,
            Path = path,
            Kind = TestKind.Unknown,
            Solvers = Array.Empty<string>(),
            ExtraFiles = Array.Empty<string>(),
            Options = new Dictionary<string, string?>(),
            Expected = Array.Empty<Expected>(),
            Markers = Array.Empty<string>(),
            SkipReason = reason,
        };

    private static TestCase InterpretTestCase(
        YamlNode root,
        string relativePath,
        string suiteName,
        TestSuite? suiteDefaults)
    {
        if (root is not YamlMapping map)
            throw new YamlParseException("expected !Test mapping at document root", root.Line, root.Column);

        TestKind kind = TestKind.Solve;
        IReadOnlyList<string>? solvers = null;
        IReadOnlyList<string>? checkAgainst = null;
        IReadOnlyList<string>? extraFiles = null;
        IReadOnlyDictionary<string, string?>? options = null;
        IReadOnlyList<string>? markers = null;
        string? name = null;
        var expected = new List<Expected>();
        string? skipReason = null;

        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "solvers":
                    solvers = ExpectStringList(entry.Value);
                    break;
                case "check_against":
                    checkAgainst = ExpectStringList(entry.Value);
                    break;
                case "extra_files":
                    extraFiles = ExpectStringList(entry.Value);
                    break;
                case "options":
                    options = ExpectStringDict(entry.Value);
                    break;
                case "markers":
                    markers = ExpectStringList(entry.Value);
                    break;
                case "name":
                    name = entry.Value is YamlScalar n ? n.Value : null;
                    break;
                case "type":
                    if (entry.Value is YamlScalar t)
                    {
                        kind = t.Value switch
                        {
                            "compile" => TestKind.Compile,
                            "output-model" => TestKind.OutputModel,
                            _ => TestKind.Solve,
                        };
                    }
                    break;
                case "expected":
                    InterpretExpected(entry.Value, expected, ref kind, ref skipReason);
                    break;
            }
        }

        // Merge suite-level defaults (solvers, options) where the case didn't set them.
        if ((solvers is null || solvers.Count == 0) && suiteDefaults?.Solvers is { Count: > 0 } sd)
            solvers = sd;

        if (suiteDefaults?.Options is { Count: > 0 } sopts)
        {
            var merged = new Dictionary<string, string?>(options ?? new Dictionary<string, string?>());
            foreach (var kv in sopts)
                if (!merged.ContainsKey(kv.Key))
                    merged[kv.Key] = kv.Value;
            options = merged;
        }

        // If options contains all_solutions, promote the expected to an
        // ExpectedAllSolutions list. The actual `-a` flag is added by OptionRenderer.
        if (options is not null && options.TryGetValue("all_solutions", out var allSol)
            && string.Equals(allSol, "true", StringComparison.OrdinalIgnoreCase))
        {
            var sols = new List<Solution>();
            bool promote = false;
            foreach (var e in expected)
            {
                switch (e)
                {
                    case ExpectedSolution s: sols.Add(s.Solution); promote = true; break;
                    case ExpectedSolutionSet ss: sols.AddRange(ss.Solutions); promote = true; break;
                    case ExpectedAllSolutions all: sols.AddRange(all.Solutions); promote = true; break;
                    default: promote = false; break;
                }
                if (!promote) break;
            }
            if (promote)
            {
                expected.Clear();
                expected.Add(new ExpectedAllSolutions(sols));
            }
        }

        if (checkAgainst is { Count: > 0 })
        {
            kind = TestKind.CheckAgainst;
            expected.Clear();
            expected.Add(new ExpectedCheckAgainst(checkAgainst));
        }

        return new TestCase
        {
            Suite = suiteName,
            Path = relativePath,
            Kind = kind,
            Solvers = solvers ?? Array.Empty<string>(),
            CheckAgainstSolvers = checkAgainst,
            ExtraFiles = extraFiles ?? Array.Empty<string>(),
            Options = options ?? new Dictionary<string, string?>(),
            Args = null, // OptionRenderer fills this in later
            Expected = expected,
            Markers = markers ?? Array.Empty<string>(),
            Name = name,
            SkipReason = skipReason,
        };
    }

    private static void InterpretExpected(
        YamlNode node,
        List<Expected> output,
        ref TestKind kind,
        ref string? skipReason)
    {
        // Untagged sequence: list of !Result / !Error etc.
        if (node is YamlSequence seq && seq.Tag is null)
        {
            foreach (var item in seq.Items)
                InterpretExpected(item, output, ref kind, ref skipReason);
            return;
        }

        // Untagged null/empty value
        if (node is YamlScalar { Tag: null } empty && empty.IsNull)
            return;

        // Dispatch by tag, regardless of node kind.
        string? tag = node.Tag;
        switch (tag)
        {
            case "!Result":
                if (node is YamlMapping rmap)
                    InterpretResult(rmap, output, ref kind);
                else if (node is YamlSequence rseq)
                {
                    var sols = new List<Solution>();
                    foreach (var item in rseq.Items)
                        if (item is YamlMapping sm)
                            sols.Add(InterpretSolution(sm));
                    output.Add(new ExpectedSolutionSet(sols));
                }
                else
                    output.Add(new ExpectedSolution(new Solution(new Dictionary<string, SolutionValue>())));
                return;
            case "!Error":
                if (node is YamlMapping emap)
                    output.Add(InterpretError(emap));
                else
                    output.Add(new ExpectedError(ErrorKind.Generic, null, null));
                return;
            case "!FlatZinc":
            case "!FlatZincJSON":
                kind = TestKind.Compile;
                output.Add(new ExpectedFlatZinc(node is YamlScalar fs ? fs.Value : ""));
                return;
            case "!OutputModel":
                kind = TestKind.OutputModel;
                output.Add(new ExpectedOutputModel(node is YamlScalar os ? os.Value : ""));
                return;
        }

        skipReason = $"unsupported `expected:` shape at line {node.Line}";
    }

    private static void InterpretResult(YamlMapping map, List<Expected> output, ref TestKind kind)
    {
        string? status = null;
        YamlNode? solutionNode = null;
        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "status":
                    if (entry.Value is YamlScalar s)
                        status = s.Value;
                    break;
                case "solution":
                    solutionNode = entry.Value;
                    break;
            }
        }

        if (string.Equals(status, "UNSATISFIABLE", StringComparison.OrdinalIgnoreCase))
        {
            output.Add(new ExpectedUnsatisfiable());
            return;
        }

        if (solutionNode is null)
        {
            // Could be just `status: SATISFIED` with no body — emit an empty solution
            output.Add(new ExpectedSolution(new Solution(new Dictionary<string, SolutionValue>())));
            return;
        }

        if (solutionNode is YamlSequence sset && solutionNode.Tag == "!SolutionSet")
        {
            var sols = new List<Solution>();
            foreach (var s in sset.Items)
                if (s is YamlMapping sm)
                    sols.Add(InterpretSolution(sm));
            if (string.Equals(status, "ALL_SOLUTIONS", StringComparison.OrdinalIgnoreCase))
                output.Add(new ExpectedAllSolutions(sols));
            else
                output.Add(new ExpectedSolutionSet(sols));
            return;
        }

        if (solutionNode is YamlMapping solMap)
        {
            output.Add(new ExpectedSolution(InterpretSolution(solMap)));
            return;
        }

        // Empty solution placeholder, e.g. `solution: !SolutionSet` with no value.
        output.Add(new ExpectedSolution(new Solution(new Dictionary<string, SolutionValue>())));
    }

    private static Solution InterpretSolution(YamlMapping map)
    {
        var vars = new Dictionary<string, SolutionValue>(StringComparer.Ordinal);
        foreach (var entry in map.Entries)
            vars[entry.Key.Value] = InterpretSolutionValue(entry.Value);
        return new Solution(vars);
    }

    private static ExpectedError InterpretError(YamlMapping map)
    {
        string? type = null;
        string? message = null;
        string? regex = null;
        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "type": if (entry.Value is YamlScalar t) type = t.Value; break;
                case "message": if (entry.Value is YamlScalar m) message = m.Value; break;
                case "regex": if (entry.Value is YamlScalar r) regex = r.Value; break;
            }
        }
        ErrorKind kind = type switch
        {
            "AssertionError" => ErrorKind.AssertionError,
            "EvaluationError" => ErrorKind.EvaluationError,
            "MiniZincError" => ErrorKind.MiniZincError,
            "TypeError" => ErrorKind.TypeError,
            "SyntaxError" => ErrorKind.SyntaxError,
            _ => ErrorKind.Generic,
        };
        return new ExpectedError(kind, message, regex);
    }

    private static SolutionValue InterpretSolutionValue(YamlNode node)
    {
        // Handle tag-driven modifiers first.
        switch (node)
        {
            case YamlScalar s when s.Tag == "!Approx":
                return new ApproxVal(ParseNumeric(s.Value, s.Line, s.Column));
            case YamlScalar s when s.Tag == "!Range":
                return ParseRange(s.Value, s.Line, s.Column);
            case YamlScalar s when s.Tag == "!Trim":
                return new TrimmedString(s.Value);
            case YamlSequence seq when seq.Tag == "!Unordered":
                return new UnorderedVal(SeqToArrayVal(seq));
            case YamlSequence seq when seq.Tag == "!!set" || seq.Tag == "!set":
                return new SetVal(seq.Items.Select(InterpretSolutionValue).ToList());
            case YamlMapping flowSet when flowSet.Tag == "!!set" || flowSet.Tag == "!set":
                // Flow-style set: keys are members; values are the same as keys (placeholder).
                return new SetVal(flowSet.Entries.Select(e => InterpretSolutionValue(e.Key)).ToList());
            case YamlMapping m when m.Tag is "!ConstrEnum"
                or "tag:yaml.org,2002:python/object:minizinc.types.ConstrEnum"
                or "!!python/object:minizinc.types.ConstrEnum":
                return InterpretConstrEnum(m);
            case YamlMapping m when m.Tag == "!AnonEnum":
                return InterpretAnonEnum(m);
        }

        switch (node)
        {
            case YamlScalar s:
                return InterpretScalarValue(s);
            case YamlSequence seq:
                return SeqToArrayVal(seq);
            case YamlMapping m:
                {
                    var fields = new Dictionary<string, SolutionValue>(StringComparer.Ordinal);
                    foreach (var entry in m.Entries)
                        fields[entry.Key.Value] = InterpretSolutionValue(entry.Value);
                    return new RecordVal(fields);
                }
        }
        throw new YamlParseException("unsupported value shape", node.Line, node.Column);
    }

    private static SolutionValue InterpretScalarValue(YamlScalar s)
    {
        if (s.Style is ScalarStyle.SingleQuoted or ScalarStyle.DoubleQuoted)
            return new StringVal(s.Value);
        if (s.Style is ScalarStyle.Literal or ScalarStyle.Folded)
            return new StringVal(s.Value);
        // Plain scalar — try bool, int, float, fall back to string
        if (s.Value == "true") return new BoolVal(true);
        if (s.Value == "false") return new BoolVal(false);
        if (s.IsNull) return new StringVal("<>");
        if (long.TryParse(s.Value, NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long i))
            return new IntVal(i);
        if (decimal.TryParse(s.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal d))
            return new FloatVal(d);
        return new StringVal(s.Value);
    }

    private static SolutionValue ParseNumeric(string raw, int line, int column)
    {
        if (long.TryParse(raw, NumberStyles.Integer | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out long i))
            return new IntVal(i);
        if (decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal d))
            return new FloatVal(d);
        throw new YamlParseException($"could not parse '{raw}' as a number", line, column);
    }

    private static SolutionValue ParseRange(string raw, int line, int column)
    {
        // !Range a..b
        int dotdot = raw.IndexOf("..", StringComparison.Ordinal);
        if (dotdot < 0)
            throw new YamlParseException($"!Range expected 'a..b', got '{raw}'", line, column);
        string lo = raw.Substring(0, dotdot).Trim();
        string hi = raw.Substring(dotdot + 2).Trim();
        return new RangeVal(ParseNumeric(lo, line, column), ParseNumeric(hi, line, column));
    }

    private static SolutionValue SeqToArrayVal(YamlSequence seq)
    {
        // Detect array of arrays for multi-dimensional inference.
        var items = seq.Items.Select(InterpretSolutionValue).ToList();
        // 2d: all items are ArrayVal of dim 1, same length
        if (items.Count > 0
            && items.All(v => v is ArrayVal a && a.Dimensionality == 1))
        {
            var firstLen = ((ArrayVal)items[0]).Shape[0];
            if (items.All(v => ((ArrayVal)v).Shape[0] == firstLen))
            {
                var flat = items.SelectMany(v => ((ArrayVal)v).Elements).ToList();
                return new ArrayVal(2, new[] { items.Count, firstLen }, flat);
            }
        }
        // 3d: all items are ArrayVal of dim 2, same shape
        if (items.Count > 0
            && items.All(v => v is ArrayVal a && a.Dimensionality == 2))
        {
            var s0 = ((ArrayVal)items[0]).Shape;
            if (items.All(v => ((ArrayVal)v).Shape.SequenceEqual(s0)))
            {
                var flat = items.SelectMany(v => ((ArrayVal)v).Elements).ToList();
                return new ArrayVal(3, new[] { items.Count, s0[0], s0[1] }, flat);
            }
        }
        return new ArrayVal(1, new[] { items.Count }, items);
    }

    private static SolutionValue InterpretConstrEnum(YamlMapping map)
    {
        string ctor = "";
        SolutionValue? arg = null;
        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "constructor": if (entry.Value is YamlScalar s) ctor = s.Value; break;
                case "argument": arg = InterpretSolutionValue(entry.Value); break;
            }
        }
        return new EnumVal(ctor, arg);
    }

    private static SolutionValue InterpretAnonEnum(YamlMapping map)
    {
        string ename = "";
        SolutionValue idx = new IntVal(0);
        foreach (var entry in map.Entries)
        {
            switch (entry.Key.Value)
            {
                case "enumName": if (entry.Value is YamlScalar s) ename = s.Value; break;
                case "value": idx = InterpretSolutionValue(entry.Value); break;
            }
        }
        return new AnonEnumVal(ename, idx);
    }

    // -------- Helpers --------

    private static IReadOnlyList<string> ExpectStringList(YamlNode node)
    {
        if (node is YamlSequence seq)
            return seq.Items.OfType<YamlScalar>().Select(s => s.Value).ToList();
        if (node is YamlScalar s && s.IsNull)
            return Array.Empty<string>();
        return Array.Empty<string>();
    }

    private static IReadOnlyDictionary<string, string?> ExpectStringDict(YamlNode node)
    {
        var d = new Dictionary<string, string?>();
        if (node is YamlMapping map)
        {
            foreach (var entry in map.Entries)
                d[entry.Key.Value] = entry.Value is YamlScalar s ? s.Value : null;
        }
        return d;
    }

    private static bool ExpectBool(YamlNode node) =>
        node is YamlScalar s && string.Equals(s.Value, "true", StringComparison.OrdinalIgnoreCase);
}
