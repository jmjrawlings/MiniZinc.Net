namespace MiniZinc.TestSpec.Model;

public abstract record SolutionValue;

public sealed record IntVal(long Value) : SolutionValue;

public sealed record FloatVal(decimal Value) : SolutionValue;

public sealed record BoolVal(bool Value) : SolutionValue;

public sealed record StringVal(string Value) : SolutionValue;

public sealed record ArrayVal(
    int Dimensionality,
    IReadOnlyList<int> Shape,
    IReadOnlyList<SolutionValue> Elements
) : SolutionValue;

public sealed record TupleVal(IReadOnlyList<SolutionValue> Fields) : SolutionValue;

public sealed record RecordVal(IReadOnlyDictionary<string, SolutionValue> Fields) : SolutionValue;

public sealed record SetVal(IReadOnlyList<SolutionValue> Elements) : SolutionValue;

public sealed record RangeVal(SolutionValue Lo, SolutionValue Hi) : SolutionValue;

public sealed record EnumVal(string Constructor, SolutionValue? Argument) : SolutionValue;

public sealed record AnonEnumVal(string EnumName, SolutionValue Index) : SolutionValue;

public sealed record ApproxVal(
    SolutionValue Inner,
    decimal? AbsEps = null,
    decimal? RelEps = null
) : SolutionValue;

public sealed record UnorderedVal(SolutionValue Inner) : SolutionValue;

public sealed record TrimmedString(string Value) : SolutionValue;
