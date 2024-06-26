namespace MiniZinc.Parser;

using Syntax;

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

/// <summary>
/// Result of parsing a minizinc model file (.mzn) or string.
/// The value of the Model field will always be present
/// even if an error occured.  In the case of an error
/// the model will contain all of the successfully parsed
/// statements up until that point.
/// </summary>
public sealed record ModelParseResult : ParseResult
{
    public required ModelSyntax Model { get; init; }
}

/// <summary>
/// Result of parsing a minizinc data file (.dzn) or string.
/// The `Data` field will always be present even if an error occured.
/// In the case of a parser error, the DataSyntax will contain all
/// of the successfully parsed variables up until that point.
/// </summary>
public sealed record DataParseResult : ParseResult
{
    public required DataSyntax Data { get; init; }
}
