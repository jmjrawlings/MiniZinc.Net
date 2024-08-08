namespace MiniZinc.Parser;

public record ParseResult
{
    public required string? SourceFile { get; init; }
    public required string SourceText { get; init; }
    public required bool Ok { get; init; }
    public required TimeSpan Elapsed { get; init; }
    public required Token FinalToken { get; init; }
    public required string? ErrorMessage { get; init; }
    public required string? ErrorTrace { get; init; }

    public void EnsureOk()
    {
        if (Ok)
            return;

        throw new MiniZincParseException(ErrorMessage ?? "", FinalToken, ErrorTrace);
    }
}
