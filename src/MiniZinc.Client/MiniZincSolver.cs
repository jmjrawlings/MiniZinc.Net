namespace MiniZinc.Client;

using System.Text.Json.Nodes;

/// <summary>
/// Solver info as provided by `minizinc --solvers`
/// </summary>
public sealed class MiniZincSolver
{
    public const string GECODE = "gecode";

    public const string CHUFFED = "chuffed";

    public required string Id { get; init; }
    public required string Name { get; init; }

    public string? Description { get; init; }

    public required string Version { get; init; }

    public string? MznLib { get; init; }

    public int? MznLibVersion { get; init; }

    public string? Executable { get; init; }

    public bool SupportsMzn { get; init; }

    public bool SupportsFzn { get; init; }

    public bool SupportsNL { get; init; }

    public bool NeedsSolns2Out { get; init; }

    public bool NeedsMznExecutable { get; init; }

    public bool NeedsStdlibDir { get; init; }

    public bool NeedsPathsFile { get; init; }

    public bool IsGUIApplication { get; init; }

    public JsonObject? ExtraInfo { get; init; }

    public IReadOnlyList<string>? Tags { get; init; }

    public IReadOnlyList<string>? StdFlags { get; init; }

    public IReadOnlyList<string>? RequiredFlags { get; init; }

    public IReadOnlyList<IReadOnlyList<string>>? ExtraFlags { get; init; }

    public override string ToString() => $"{Name}  v{Version}";
}
