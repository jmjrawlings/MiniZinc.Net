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
}
