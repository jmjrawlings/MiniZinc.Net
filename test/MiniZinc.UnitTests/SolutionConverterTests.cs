using MiniZinc.Parser;
using MiniZinc.TestRunner;
using MiniZinc.TestSpec.Model;

public sealed class SolutionConverterTests
{
    private static Solution Sol(params (string, SolutionValue)[] vars)
    {
        Dictionary<string, SolutionValue> d = new();
        foreach ((string name, SolutionValue val) in vars)
            d[name] = val;
        return new Solution(d);
    }

    [Fact]
    public void converts_scalars()
    {
        MiniZincData data = SolutionConverter.FromSolution(
            Sol(("i", new IntVal(3)), ("f", new FloatVal(1.5m)), ("b", new BoolVal(true)))
        );
        data.Get<IntExpr>("i").Value.ShouldBe(3);
        data.Get<FloatExpr>("f").Value.ShouldBe(1.5m);
        data.Get<BoolExpr>("b").Value.ShouldBeTrue();
    }

    [Fact]
    public void converts_array_and_set()
    {
        MiniZincData data = SolutionConverter.FromSolution(
            Sol(
                ("a", new ArrayVal(1, [3], [new IntVal(1), new IntVal(2), new IntVal(3)])),
                ("s", new SetVal([new IntVal(1), new IntVal(2)]))
            )
        );
        data.Get<Array1dExpr>("a").Elements!.Count.ShouldBe(3);
        data.Get<SetExpr>("s").Elements!.Count.ShouldBe(2);
    }

    [Fact]
    public void round_trips_through_comparer()
    {
        // The converter output must compare equal to the same values parsed as DZN.
        MiniZincData expected = SolutionConverter.FromSolution(
            Sol(("x", new IntVal(42)), ("a", new ArrayVal(1, [2], [new IntVal(1), new IntVal(2)])))
        );
        Parser.TryParseDataString("x = 42;\na = [1, 2];", out MiniZincData? actual, out _, out _, out _)
            .ShouldBeTrue();
        SolutionComparer.Compare(expected, actual!).IsMatch.ShouldBeTrue();
    }

    [Fact]
    public void converts_record()
    {
        MiniZincData data = SolutionConverter.FromSolution(
            Sol(
                (
                    "r",
                    new RecordVal(
                        new Dictionary<string, SolutionValue> { ["a"] = new IntVal(1), ["b"] = new IntVal(2) }
                    )
                )
            )
        );
        data.Get<RecordExpr>("r").Fields.Count.ShouldBe(2);
    }

    [Fact]
    public void try_from_solution_reports_unrenderable()
    {
        // A solution with no variables renders to null → TryFromSolution fails cleanly.
        bool ok = SolutionConverter.TryFromSolution(Sol(), out _, out string? error);
        ok.ShouldBeFalse();
        error.ShouldNotBeNull();
    }
}
