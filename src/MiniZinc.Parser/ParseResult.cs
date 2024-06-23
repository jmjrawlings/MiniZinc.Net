﻿using MiniZinc.Parser.Syntax;

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
        if (ErrorMessage is not null)
            throw new ParseException(ErrorMessage, ErrorTrace);
        else
            throw new ParseException("Could not parse");
    }
}

public sealed record ModelParseResult : ParseResult
{
    public required ModelSyntax Model { get; init; }
}

public sealed record DataParseResult : ParseResult
{
    public required DataSyntax Data { get; init; }
}
