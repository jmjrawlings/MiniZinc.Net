namespace MiniZinc.TestSpec;

using MiniZinc.TestSpec.Model;
using MiniZinc.TestSpec.Yaml;

public interface ISkipList
{
    /// Returns a skip reason if the given (path, solver) should be skipped; null otherwise.
    string? Reason(string testRelativePath, string? solver);
}

public sealed class EmptySkipList : ISkipList
{
    public static readonly EmptySkipList Instance = new();
    public string? Reason(string testRelativePath, string? solver) => null;
}

/// <summary>
/// Loads <c>spec/skip-list.yml</c> if present. Format:
/// <code>
/// skips:
///   - path: unit/types/var_string_a.mzn   # exact path or simple glob
///     reason: var string unsupported
///     solver: highs                        # optional solver scope
/// </code>
/// </summary>
public sealed class YamlSkipList : ISkipList
{
    private readonly List<Entry> _entries;

    private sealed record Entry(string Pattern, string? Solver, string Reason);

    private YamlSkipList(List<Entry> entries) => _entries = entries;

    public static ISkipList LoadOrEmpty(FileInfo file)
    {
        if (!file.Exists)
            return EmptySkipList.Instance;
        try
        {
            string text = File.ReadAllText(file.FullName);
            var docs = new YamlScanner(text, file.Name).ParseStream();
            if (docs.Count == 0 || docs[0] is not YamlMapping root)
                return EmptySkipList.Instance;

            var entries = new List<Entry>();
            foreach (var topEntry in root.Entries)
            {
                if (topEntry.Key.Value != "skips" || topEntry.Value is not YamlSequence seq)
                    continue;
                foreach (var item in seq.Items)
                {
                    if (item is not YamlMapping em)
                        continue;
                    string? path = null;
                    string? solver = null;
                    string? reason = null;
                    foreach (var e in em.Entries)
                    {
                        switch (e.Key.Value)
                        {
                            case "path": if (e.Value is YamlScalar p) path = p.Value; break;
                            case "solver": if (e.Value is YamlScalar sv) solver = sv.Value; break;
                            case "reason": if (e.Value is YamlScalar r) reason = r.Value; break;
                        }
                    }
                    if (path is null || reason is null)
                        continue;
                    entries.Add(new Entry(path, solver, reason));
                }
            }
            return new YamlSkipList(entries);
        }
        catch
        {
            return EmptySkipList.Instance;
        }
    }

    public string? Reason(string testRelativePath, string? solver)
    {
        foreach (var entry in _entries)
        {
            if (!MatchesPath(entry.Pattern, testRelativePath))
                continue;
            if (entry.Solver is not null && !string.Equals(entry.Solver, solver, StringComparison.OrdinalIgnoreCase))
                continue;
            return entry.Reason;
        }
        return null;
    }

    private static bool MatchesPath(string pattern, string path)
    {
        // Simple '*' glob within a single segment. Supports patterns like
        // "unit/regression/checker_*.mzn".
        // Convert to a regex.
        var re = new System.Text.RegularExpressions.Regex(
            "^" + System.Text.RegularExpressions.Regex.Escape(pattern).Replace("\\*", "[^/]*") + "$",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant
        );
        return re.IsMatch(path);
    }
}
