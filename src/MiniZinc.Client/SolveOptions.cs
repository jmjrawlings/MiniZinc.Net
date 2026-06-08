namespace MiniZinc.Client;

/// <summary>
/// Options controlling a single MiniZinc solve/compile invocation. Lets callers
/// pass data files, extra model files, search paths and raw arguments directly to
/// the executable instead of munging them into the model — see the integration
/// harness redesign (<c>.claude/harness-redesign.md</c>).
/// </summary>
public sealed record SolveOptions
{
    /// <summary>
    /// Solver id or name (eg <c>org.minizinc.gecode</c> or <c>gecode</c>). When
    /// null the Gecode default is used. It is an error to also specify a
    /// <c>--solver</c> flag via <see cref="ExtraArgs"/>.
    /// </summary>
    public string? Solver { get; init; }

    /// <summary>
    /// Data files (<c>.dzn</c>/<c>.json</c>/<c>.mpc</c>) passed to minizinc as file
    /// arguments. JSON/MPC data is coerced type-directed by minizinc itself, so it
    /// cannot be inlined into the model the way plain DZN can.
    /// </summary>
    public IReadOnlyList<string>? DataFiles { get; init; }

    /// <summary>
    /// Extra model files (<c>.mzn</c>) passed to minizinc as additional file
    /// arguments alongside the serialized model.
    /// </summary>
    public IReadOnlyList<string>? ExtraModelFiles { get; init; }

    /// <summary>
    /// Raw command-line fragments appended verbatim (each parsed as a command
    /// line). Used by the legacy <c>params</c> overloads and for one-off flags.
    /// </summary>
    public IReadOnlyList<string>? ExtraArgs { get; init; }

    /// <summary>
    /// Search paths added via <c>-I</c>.
    /// </summary>
    public IReadOnlyList<string>? SearchPaths { get; init; }

    /// <summary>
    /// When set, the solve is cancelled after this duration. Implemented as a
    /// cancellation-token timeout linked to the caller's token (not minizinc's own
    /// <c>--time-limit</c>), so the whole solver process tree is killed on expiry.
    /// </summary>
    public TimeSpan? Timeout { get; init; }

    /// <summary>
    /// Compile only (<c>-c</c>), do not solve. Full FlatZinc-compile handling
    /// lands in a later phase; this currently only wires the flag and suppresses
    /// the JSON-stream solve flags.
    /// </summary>
    public bool CompileOnly { get; init; }

    /// <summary>
    /// Output file for compilation (<c>-o</c>), used together with
    /// <see cref="CompileOnly"/>.
    /// </summary>
    public string? OutputFile { get; init; }
}
