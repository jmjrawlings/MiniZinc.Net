/// <summary>
/// Shortcuts for the ambient test environment. Exposed via `global using static`
/// so call sites can write `Cancellation` instead of the full
/// `TestContext.Current.CancellationToken`.
/// </summary>
public static class TestEnv
{
    /// <summary>
    /// The current test's cancellation token (fired on timeout or run abort).
    /// </summary>
    public static CancellationToken Cancellation => TestContext.Current.CancellationToken;
}
