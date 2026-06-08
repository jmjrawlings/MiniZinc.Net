namespace MiniZinc.TestRunner;

using System.Globalization;
using System.Text;
using MiniZinc.Parser;

/// <summary>Result of comparing two solutions. <see cref="Diff"/> is non-null only on a miss.</summary>
public sealed record CompareResult(bool IsMatch, string? Diff)
{
    public static readonly CompareResult Match = new(true, null);

    public static CompareResult Fail(string diff) => new(false, diff);
}

/// <summary>Tolerances for solution comparison.</summary>
public sealed record CompareOptions
{
    /// <summary>Absolute float tolerance: |e - a| &lt;= this passes.</summary>
    public decimal FloatAbsTol { get; init; } = 0.000001m;

    /// <summary>Relative float tolerance: |e - a| &lt;= this * max(|e|,|a|) passes.</summary>
    public decimal FloatRelTol { get; init; } = 0.000001m;

    public static readonly CompareOptions Default = new();
}

/// <summary>
/// Compares an expected solution against an actual one, both as
/// <see cref="MiniZincData"/> (see <see cref="TestCaseSolutionConverter"/>). This replaces
/// the hand-rolled <c>Check</c> family in the old integration harness; each rule
/// below fixes a defect that one had (float <c>==</c>, silently-skipped missing
/// keys, no unordered sets, no enum equivalence).
///
/// Because the expected side comes through <see cref="TestCaseSolutionConverter"/> (which
/// flattens <c>!Approx</c>/<c>!Unordered</c> annotations and array shape), float
/// tolerance and unordered-set semantics are applied <em>universally</em>, and
/// arrays are compared by row-major element list rather than declared shape.
/// </summary>
public static class MiniZincDataComparer
{
    /// <summary>Actual matches expected. Expected keys must all be present (missing = FAIL,
    /// except an expected <c>&lt;&gt;</c>/absent); surplus actual keys are ignored.</summary>
    public static CompareResult Compare(
        MiniZincData expected,
        MiniZincData actual,
        CompareOptions? options = null
    )
    {
        CompareOptions opts = options ?? CompareOptions.Default;
        foreach (KeyValuePair<string, MiniZincExpr> kv in expected)
        {
            string name = kv.Key;
            // Internal/output pseudo-vars are not solution variables.
            if (name.StartsWith('_'))
                continue;

            MiniZincExpr e = kv.Value;
            if (!actual.TryGetValue(name, out MiniZincExpr? a))
            {
                // An absent optional (<>) legitimately maps to a missing key.
                if (IsAbsent(e))
                    continue;
                return CompareResult.Fail($"{name}: missing from actual solution");
            }

            if (!CompareExpr(e, a, opts, name, out string? diff))
                return CompareResult.Fail(diff!);
        }

        return CompareResult.Match;
    }

    /// <summary>Actual matches ANY of the expected solutions. Diff describes the closest/last miss.</summary>
    public static CompareResult CompareAny(
        IReadOnlyList<MiniZincData> expected,
        MiniZincData actual,
        CompareOptions? options = null
    )
    {
        if (expected.Count == 0)
            return CompareResult.Fail("no expected solutions to compare against");

        string? lastDiff = null;
        foreach (MiniZincData candidate in expected)
        {
            CompareResult r = Compare(candidate, actual, options);
            if (r.IsMatch)
                return CompareResult.Match;
            lastDiff = r.Diff;
        }

        return CompareResult.Fail(
            $"actual matched none of the {expected.Count} expected solutions; last miss: {lastDiff}"
        );
    }

    private static bool CompareExpr(
        MiniZincExpr e,
        MiniZincExpr a,
        CompareOptions o,
        string path,
        out string? diff
    )
    {
        diff = null;

        // Unwrap a single indexed element (`i: v`) on either side.
        if (e is IndexedExpr ie)
            e = ie.Value;
        if (a is IndexedExpr ia)
            a = ia.Value;

        switch (e, a)
        {
            // Optional / <>.
            case var (x, y) when IsAbsent(x) && IsAbsent(y):
                return true;

            // Numbers — int exact, anything involving a float uses tolerance.
            case (IntExpr ei, IntExpr ai):
                return Eq(ei.Value == ai.Value, path, ei.Value, ai.Value, out diff);
            case (IntExpr ei, FloatExpr af):
                return FloatEq(ei.Value, af.Value, o, path, out diff);
            case (FloatExpr ef, IntExpr ai):
                return FloatEq(ef.Value, ai.Value, o, path, out diff);
            case (FloatExpr ef, FloatExpr af):
                return FloatEq(ef.Value, af.Value, o, path, out diff);

            case (BoolExpr eb, BoolExpr ab):
                return Eq(eb.Value == ab.Value, path, eb.Value, ab.Value, out diff);

            case (StringExpr es, StringExpr astr):
                return Eq(es.Value == astr.Value, path, es.Value, astr.Value, out diff);

            // Sets — unordered.
            case (SetExpr es2, SetExpr as2):
                return CompareUnordered(Elems(es2.Elements), Elems(as2.Elements), o, path, out diff);

            // Records — strict field set, recurse.
            case (RecordExpr er, RecordExpr ar):
                return CompareRecord(er, ar, o, path, out diff);

            // Enum representation equivalence (Foo(1) ≡ to_enum(Foo,1), bare member names).
            case (CallExpr or IdentExpr, CallExpr or IdentExpr) when TryEnum(e, out var ee) && TryEnum(a, out var ae):
                return Eq(ee == ae, path, ee, ae, out diff);

            // Everything sequence-like (arrays in any form, tuples, and the
            // tuple↔array1d cross-cases) compares by ordered element list.
            default:
                IReadOnlyList<MiniZincExpr>? eseq = AsSequence(e);
                IReadOnlyList<MiniZincExpr>? aseq = AsSequence(a);
                if (eseq is not null && aseq is not null)
                    return CompareOrdered(eseq, aseq, o, path, out diff);

                diff = $"{path}: expected {Describe(e)} but was {Describe(a)}";
                return false;
        }
    }

    private static bool CompareOrdered(
        IReadOnlyList<MiniZincExpr> e,
        IReadOnlyList<MiniZincExpr> a,
        CompareOptions o,
        string path,
        out string? diff
    )
    {
        e = Order(e);
        a = Order(a);
        if (e.Count != a.Count)
        {
            diff = $"{path}: expected {e.Count} elements but was {a.Count}";
            return false;
        }

        for (int i = 0; i < e.Count; i++)
            if (!CompareExpr(e[i], a[i], o, $"{path}[{i}]", out diff))
                return false;

        diff = null;
        return true;
    }

    private static bool CompareUnordered(
        IReadOnlyList<MiniZincExpr> e,
        IReadOnlyList<MiniZincExpr> a,
        CompareOptions o,
        string path,
        out string? diff
    )
    {
        if (e.Count != a.Count)
        {
            diff = $"{path}: expected set of {e.Count} elements but was {a.Count}";
            return false;
        }

        bool[] used = new bool[a.Count];
        foreach (MiniZincExpr ex in e)
        {
            bool found = false;
            for (int j = 0; j < a.Count; j++)
            {
                if (used[j])
                    continue;
                if (CompareExpr(ex, a[j], o, path, out _))
                {
                    used[j] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                diff = $"{path}: expected element {Describe(ex)} not found in actual set";
                return false;
            }
        }

        diff = null;
        return true;
    }

    private static bool CompareRecord(
        RecordExpr e,
        RecordExpr a,
        CompareOptions o,
        string path,
        out string? diff
    )
    {
        Dictionary<string, MiniZincExpr> af = new(StringComparer.Ordinal);
        foreach ((IdentExpr key, MiniZincExpr val) in a.Fields)
            af[key.ToString()] = val;

        foreach ((IdentExpr ekey, MiniZincExpr eval) in e.Fields)
        {
            string name = ekey.ToString();
            if (!af.TryGetValue(name, out MiniZincExpr? aval))
            {
                if (IsAbsent(eval))
                    continue;
                diff = $"{path}.{name}: missing field in actual record";
                return false;
            }

            if (!CompareExpr(eval, aval, o, $"{path}.{name}", out diff))
                return false;
        }

        diff = null;
        return true;
    }

    // ---- helpers -------------------------------------------------------------

    private static bool Eq<T>(bool equal, string path, T e, T a, out string? diff)
    {
        if (equal)
        {
            diff = null;
            return true;
        }
        diff = $"{path}: expected {e} but was {a}";
        return false;
    }

    private static bool FloatEq(decimal e, decimal a, CompareOptions o, string path, out string? diff)
    {
        decimal d = Math.Abs(e - a);
        if (d <= o.FloatAbsTol || d <= o.FloatRelTol * Math.Max(Math.Abs(e), Math.Abs(a)))
        {
            diff = null;
            return true;
        }
        diff = $"{path}: expected {Fmt(e)} but was {Fmt(a)} (|Δ|={Fmt(d)} exceeds tolerance)";
        return false;
    }

    private static bool IsAbsent(MiniZincExpr e) =>
        e is EmptyExpr || (e is StringExpr s && s.Value == "<>");

    private static IReadOnlyList<MiniZincExpr> Elems(IReadOnlyList<MiniZincExpr>? xs) =>
        xs ?? [];

    /// <summary>Element list of any array-like form, else null. Tuples are sequence-like too.</summary>
    private static IReadOnlyList<MiniZincExpr>? AsSequence(MiniZincExpr e) =>
        e switch
        {
            // Array1d/2d/3dExpr all derive from ArrayExpr and store flat Elements.
            ArrayExpr arr => Elems(arr.Elements),
            TupleExpr t => t.Fields,
            // Dedicated array1d/2d/3d constructor-call nodes (what the parser emits
            // for `array2d(1..m,1..n,[...])` etc.) — unwrap to their element list.
            Array1dCallExpr c => Elems(c.Array.Elements),
            Array2dCallExpr1d c => Elems(c.Array.Elements),
            Array2dCallExpr2d c => Elems(c.Array.Elements),
            Array3dCallExpr c => c.Elements,
            // Generic call fallback, in case array constructors ever arrive as plain calls.
            CallExpr { Name.StringValue: "array1d", Args: { Count: >= 1 } args } => AsSequence(args[^1]),
            CallExpr { Name.StringValue: "array2d", Args: { Count: >= 3 } args } => AsSequence(args[^1]),
            CallExpr { Name.StringValue: "array3d", Args: { Count: >= 4 } args } => AsSequence(args[^1]),
            _ => null
        };

    /// <summary>If a sequence is fully index-tagged (`i: v`), order by index; else keep order.</summary>
    private static IReadOnlyList<MiniZincExpr> Order(IReadOnlyList<MiniZincExpr> xs)
    {
        if (xs.Count > 0 && xs.All(x => x is IndexedExpr))
            return xs.Cast<IndexedExpr>().OrderBy(i => SortKey(i.Index)).Select(i => (MiniZincExpr)i.Value).ToList();
        return xs;
    }

    private static long SortKey(MiniZincExpr index) => index is IntExpr i ? i.Value : 0;

    /// <summary>
    /// Canonical form of an enum value, or false if not enum-shaped. Handles
    /// <c>Foo(1)</c>, <c>to_enum(Foo, 1)</c> → <c>"Foo#1"</c>, and a bare member
    /// name → the name. Member-name ↔ index equivalence needs the enum decl and is
    /// deliberately not attempted (see Phase 3 spot-check).
    /// </summary>
    private static bool TryEnum(MiniZincExpr e, out string canonical)
    {
        switch (e)
        {
            case CallExpr { Name.StringValue: "to_enum", Args: { Count: 2 } args }
                when args[1] is IntExpr idx:
                canonical = $"{EnumName(args[0])}#{idx.Value}";
                return true;
            case CallExpr { Args: { Count: 1 } args } call when args[0] is IntExpr idx:
                canonical = $"{call.Name.StringValue}#{idx.Value}";
                return true;
            case IdentExpr id:
                canonical = id.ToString();
                return true;
            default:
                canonical = "";
                return false;
        }
    }

    private static string EnumName(MiniZincExpr e) =>
        e is IdentExpr id ? id.ToString() : e.ToString() ?? "";

    private static string Fmt(decimal d) => d.ToString("0.################", CultureInfo.InvariantCulture);

    private static string Describe(MiniZincExpr e)
    {
        try
        {
            return e.ToString() ?? e.GetType().Name;
        }
        catch
        {
            return e.GetType().Name;
        }
    }
}
