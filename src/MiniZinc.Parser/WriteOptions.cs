namespace MiniZinc.Parser;

/// <summary>
/// Options that specify how models are
/// written to string or file
/// </summary>
public sealed record WriteOptions
{
    /// <summary>
    /// If true, the shortest possible output will be generated
    /// </summary>
    public bool Minify { get; init; }

    /// <summary>
    /// If true, the output will be "prettified""
    /// </summary>
    public bool Prettify { get; init; }

    /// <summary>
    /// Spaces to use for a Tab
    /// </summary>
    public int TabSize { get; init; } = 2;

    /// <summary>
    /// Spaces to use for a Tab
    /// </summary>
    public bool SkipOutput { get; init; }

    public static WriteOptions Minimal = new WriteOptions { Minify = true };

    public static WriteOptions Pretty = new WriteOptions { Prettify = true };

    public static WriteOptions Default = new WriteOptions { };
}
