using MiniZinc.Parser;
using MiniZinc.TestRunner;

public sealed class SolutionComparerTests
{
    /// Parse a DZN data fragment into MiniZincData (the same path the client uses
    /// for actual solver output and the converter for expected output).
    private static MiniZincData Data(string dzn)
    {
        Parser.TryParseDataString(dzn, out MiniZincData? d, out string? err, out string? trace, out _)
            .ShouldBeTrue(err ?? trace);
        return d!;
    }

    private static void ShouldMatch(string expected, string actual)
    {
        CompareResult r = SolutionComparer.Compare(Data(expected), Data(actual));
        r.IsMatch.ShouldBeTrue(r.Diff);
    }

    private static void ShouldMiss(string expected, string actual)
    {
        CompareResult r = SolutionComparer.Compare(Data(expected), Data(actual));
        r.IsMatch.ShouldBeFalse();
        r.Diff.ShouldNotBeNull();
    }

    [Fact]
    public void int_exact_match_and_mismatch()
    {
        ShouldMatch("x = 3;", "x = 3;");
        ShouldMiss("x = 3;", "x = 4;");
    }

    [Fact]
    public void float_within_tolerance_passes()
    {
        ShouldMatch("x = 1.0;", "x = 1.0000001;");
    }

    [Fact]
    public void float_outside_tolerance_fails()
    {
        ShouldMiss("x = 1.0;", "x = 1.1;");
    }

    [Fact]
    public void int_float_coercion()
    {
        ShouldMatch("x = 2;", "x = 2.0;");
        ShouldMatch("x = 2.0;", "x = 2;");
    }

    [Fact]
    public void missing_key_fails()
    {
        ShouldMiss("x = 1;\ny = 2;", "x = 1;");
    }

    [Fact]
    public void surplus_actual_key_ignored()
    {
        ShouldMatch("x = 1;", "x = 1;\ny = 2;");
    }

    [Fact]
    public void set_is_unordered()
    {
        ShouldMatch("s = {1, 2, 3};", "s = {3, 1, 2};");
        ShouldMiss("s = {1, 2, 3};", "s = {1, 2, 4};");
        ShouldMiss("s = {1, 2, 3};", "s = {1, 2};");
    }

    [Fact]
    public void record_match_and_missing_field_fails()
    {
        ShouldMatch("r = (a: 1, b: 2);", "r = (a: 1, b: 2);");
        ShouldMiss("r = (a: 1, b: 2);", "r = (a: 1);");
    }

    [Fact]
    public void record_field_value_mismatch_fails()
    {
        ShouldMiss("r = (a: 1, b: 2);", "r = (a: 1, b: 3);");
    }

    [Fact]
    public void tuple_match_and_tuple_array_cross()
    {
        ShouldMatch("t = (1, 2, 3);", "t = (1, 2, 3);");
        // tuple ↔ array1d interchange
        ShouldMatch("t = (1, 2);", "t = [1, 2];");
    }

    [Fact]
    public void array2d_normalizes_to_flat_elements()
    {
        // Expected comes through the converter as a flat 1d list; actual may be 2d.
        ShouldMatch("a = [1, 2, 3, 4];", "a = array2d(1..2, 1..2, [1, 2, 3, 4]);");
        ShouldMiss("a = [1, 2, 3, 4];", "a = array2d(1..2, 1..2, [1, 2, 3, 9]);");
    }

    [Fact]
    public void nested_float_in_array_uses_tolerance()
    {
        ShouldMatch("a = [1.0, 2.0];", "a = [1.0000001, 2.0];");
        ShouldMiss("a = [1.0, 2.0];", "a = [1.0, 2.5];");
    }

    [Fact]
    public void enum_form_equivalence()
    {
        // Foo(1) ≡ to_enum(Foo, 1)
        ShouldMatch("x = Foo(1);", "x = to_enum(Foo, 1);");
        ShouldMatch("x = to_enum(Foo, 1);", "x = Foo(1);");
        ShouldMiss("x = Foo(1);", "x = Foo(2);");
        ShouldMiss("x = Foo(1);", "x = to_enum(Bar, 1);");
    }

    [Fact]
    public void absent_optional_matches_missing_or_empty()
    {
        // <> ≡ <>
        ShouldMatch("x = <>;", "x = <>;");
        // expected <> with the key absent from actual is still a match
        CompareResult r = SolutionComparer.Compare(Data("x = <>;"), Data("y = 1;"));
        r.IsMatch.ShouldBeTrue(r.Diff);
    }

    [Fact]
    public void compare_any_matches_one_of_several()
    {
        MiniZincData actual = Data("x = 2;");
        List<MiniZincData> expected = [Data("x = 1;"), Data("x = 2;"), Data("x = 3;")];
        SolutionComparer.CompareAny(expected, actual).IsMatch.ShouldBeTrue();
    }

    [Fact]
    public void compare_any_fails_when_none_match()
    {
        MiniZincData actual = Data("x = 9;");
        List<MiniZincData> expected = [Data("x = 1;"), Data("x = 2;")];
        CompareResult r = SolutionComparer.CompareAny(expected, actual);
        r.IsMatch.ShouldBeFalse();
        r.Diff.ShouldNotBeNull();
    }

    [Fact]
    public void compare_any_empty_expected_fails()
    {
        SolutionComparer.CompareAny([], Data("x = 1;")).IsMatch.ShouldBeFalse();
    }
}
