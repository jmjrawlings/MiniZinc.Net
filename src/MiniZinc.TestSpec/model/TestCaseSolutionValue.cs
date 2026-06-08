namespace MiniZinc.TestSpec.Model;

public abstract record TestCaseSolutionValue;

public sealed record IntVal(long Value) : TestCaseSolutionValue;

public sealed record FloatVal(decimal Value) : TestCaseSolutionValue;

public sealed record BoolVal(bool Value) : TestCaseSolutionValue;

public sealed record StringVal(string Value) : TestCaseSolutionValue;

public sealed record ArrayVal(
    int Dimensionality,
    IReadOnlyList<int> Shape,
    IReadOnlyList<TestCaseSolutionValue> Elements
) : TestCaseSolutionValue;

public sealed record TupleVal(IReadOnlyList<TestCaseSolutionValue> Fields) : TestCaseSolutionValue;

public sealed record RecordVal(IReadOnlyDictionary<string, TestCaseSolutionValue> Fields) : TestCaseSolutionValue;

public sealed record SetVal(IReadOnlyList<TestCaseSolutionValue> Elements) : TestCaseSolutionValue;

public sealed record RangeVal(TestCaseSolutionValue Lo, TestCaseSolutionValue Hi) : TestCaseSolutionValue;

public sealed record EnumVal(string Constructor, TestCaseSolutionValue? Argument) : TestCaseSolutionValue;

public sealed record AnonEnumVal(string EnumName, TestCaseSolutionValue Index) : TestCaseSolutionValue;

public sealed record ApproxVal(
    TestCaseSolutionValue Inner,
    decimal? AbsEps = null,
    decimal? RelEps = null
) : TestCaseSolutionValue;

public sealed record UnorderedVal(TestCaseSolutionValue Inner) : TestCaseSolutionValue;

public sealed record TrimmedString(string Value) : TestCaseSolutionValue;
