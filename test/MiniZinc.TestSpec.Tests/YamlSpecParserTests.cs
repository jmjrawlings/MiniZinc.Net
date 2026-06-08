namespace MiniZinc.TestSpec.Tests;

/// <summary>
/// End-to-end interpreter tests. Each test embeds a real YAML payload pulled
/// from a libminizinc spec file and asserts the resulting <see cref="TestCase"/>
/// shape and contents.
/// </summary>
public class YamlSpecParserTests
{
    private static TestCase Interpret(string yaml, string path = "test.mzn")
    {
        var source = $"/***\n{yaml}\n***/\n";
        var tmp = Path.GetTempFileName();
        var renamed = tmp + ".mzn";
        File.Move(tmp, renamed);
        try
        {
            File.WriteAllText(renamed, source);
            var dir = new DirectoryInfo(Path.GetDirectoryName(renamed)!);
            var cases = YamlSpecParser.ParseTestCases(new FileInfo(renamed), dir, "default");
            cases.ShouldNotBeNull();
            cases!.Count.ShouldBeGreaterThan(0);
            return cases[0];
        }
        finally
        {
            if (File.Exists(renamed)) File.Delete(renamed);
        }
    }

    private static IReadOnlyList<TestCase> InterpretAll(string yaml)
    {
        var source = $"/***\n{yaml}\n***/\n";
        var tmp = Path.GetTempFileName();
        var renamed = tmp + ".mzn";
        File.Move(tmp, renamed);
        try
        {
            File.WriteAllText(renamed, source);
            var dir = new DirectoryInfo(Path.GetDirectoryName(renamed)!);
            var cases = YamlSpecParser.ParseTestCases(new FileInfo(renamed), dir, "default");
            cases.ShouldNotBeNull();
            return cases!;
        }
        finally
        {
            if (File.Exists(renamed)) File.Delete(renamed);
        }
    }

    // ----- Sample 1: from spec/unit/compilation/aggregation.mzn -----

    [Fact]
    public void test_interprets_compile_with_flatzinc_expected()
    {
        var tc = Interpret("""
            --- !Test
            type: compile
            solvers: [gecode]
            expected: !FlatZinc aggregation.fzn
            """);

        tc.Kind.ShouldBe(TestKind.Compile);
        tc.Solvers.ShouldBe(new[] { "gecode" });
        tc.Expected.Count.ShouldBe(1);
        var fz = tc.Expected[0].ShouldBeOfType<ExpectedFlatZinc>();
        fz.RelativePath.ShouldBe("aggregation.fzn");
    }

    // ----- Sample 2: from spec/unit/regression/bug570.mzn (pattern) -----

    [Fact]
    public void test_interprets_output_model_expected()
    {
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !OutputModel bug570.ozn
            """);

        tc.Kind.ShouldBe(TestKind.OutputModel);
        var om = tc.Expected[0].ShouldBeOfType<ExpectedOutputModel>();
        om.RelativePath.ShouldBe("bug570.ozn");
    }

    // ----- Sample 3: typed error with regex -----

    [Fact]
    public void test_interprets_error_with_type_and_regex()
    {
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !Error
              type: AssertionError
              regex: .*assertion failed.*
            """);

        tc.Kind.ShouldBe(TestKind.Solve);
        var err = tc.Expected[0].ShouldBeOfType<ExpectedError>();
        err.Kind.ShouldBe(ErrorKind.AssertionError);
        err.Regex.ShouldBe(".*assertion failed.*");
    }

    // ----- Sample 4: empty error body -----

    [Fact]
    public void test_interprets_empty_error()
    {
        // Pattern from spec/unit/general/test_bad_array_size-bad.mzn
        var tc = Interpret("""
            !Test
            expected: !Error
            """);

        var err = tc.Expected[0].ShouldBeOfType<ExpectedError>();
        err.Kind.ShouldBe(ErrorKind.Generic);
        err.Message.ShouldBeNull();
        err.Regex.ShouldBeNull();
    }

    // ----- Sample 5: solve with !!set value -----

    [Fact]
    public void test_interprets_solution_with_flow_set()
    {
        // Pattern from spec/unit/json/coerce_enum_str.mzn
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !Result
              solution: !TestCaseSolution
                dset: !!set {Fri, Sat, Sun}
            """);

        var sol = tc.Expected[0].ShouldBeOfType<ExpectedSolution>().TestCaseSolution;
        sol.Variables.ContainsKey("dset").ShouldBeTrue();
        var set = sol.Variables["dset"].ShouldBeOfType<SetVal>();
        set.Elements.Count.ShouldBe(3);
    }

    // ----- Sample 6: !Approx wrapping a numeric -----

    [Fact]
    public void test_interprets_approx_modifier()
    {
        // Pattern from spec/unit/general/mortgage.mzn
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !Result
              solution: !TestCaseSolution
                P: !Approx 373.0277986476333
            """);

        var sol = tc.Expected[0].ShouldBeOfType<ExpectedSolution>().TestCaseSolution;
        var approx = sol.Variables["P"].ShouldBeOfType<ApproxVal>();
        approx.Inner.ShouldBeOfType<FloatVal>().Value.ShouldBe(373.0277986476333m);
    }

    // ----- Sample 7: block-literal _output_item with !Trim -----

    [Fact]
    public void test_interprets_trim_output_item()
    {
        // Pattern from spec/unit/regression/bug259.mzn
        var tc = Interpret("""
            !Test
            expected:
            - !Result
              solution: !TestCaseSolution
                _output_item: !Trim |
                  sz[1][1] = 1
                  sz[1][2] = 0
                obj: 62
            """);

        var sol = tc.Expected[0].ShouldBeOfType<ExpectedSolution>().TestCaseSolution;
        sol.HasOutputItem.ShouldBeTrue();
        var trim = sol.Variables["_output_item"].ShouldBeOfType<TrimmedString>();
        trim.Value.ShouldContain("sz[1][1] = 1");
        trim.Value.ShouldContain("sz[1][2] = 0");
        sol.Variables["obj"].ShouldBeOfType<IntVal>().Value.ShouldBe(62);
    }

    // ----- Sample 8: !SolutionSet (multiple solutions, all-solutions mode) -----

    [Fact]
    public void test_interprets_solution_set()
    {
        // Pattern from spec/unit/general/md_exists.mzn
        var tc = Interpret("""
            --- !Test
            options:
              all_solutions: true
            solvers: [gecode]
            expected: !Result
              solution: !SolutionSet
              - !TestCaseSolution
                b: true
                bs: [1, 2]
              - !TestCaseSolution
                b: false
                bs: [3, 4]
            """);

        var all = tc.Expected[0].ShouldBeOfType<ExpectedAllSolutions>();
        all.Solutions.Count.ShouldBe(2);
        all.Solutions[0].Variables["b"].ShouldBeOfType<BoolVal>().Value.ShouldBeTrue();
        all.Solutions[0].Variables["bs"].ShouldBeOfType<ArrayVal>()
            .Dimensionality.ShouldBe(1);
    }

    // ----- Sample 9: multi-document YAML preamble -----

    [Fact]
    public void test_interprets_multi_document_preamble()
    {
        // Pattern: many spec/unit/search/*.mzn files have two --- !Test documents.
        var cases = InterpretAll("""
            --- !Test
            solvers: [gecode]
            expected: !FlatZinc one.fzn
            type: compile
            --- !Test
            solvers: [chuffed]
            expected: !FlatZinc two.fzn
            type: compile
            """);

        cases.Count.ShouldBe(2);
        cases[0].Solvers.ShouldBe(new[] { "gecode" });
        cases[1].Solvers.ShouldBe(new[] { "chuffed" });
        cases[0].Expected[0].ShouldBeOfType<ExpectedFlatZinc>().RelativePath.ShouldBe("one.fzn");
        cases[1].Expected[0].ShouldBeOfType<ExpectedFlatZinc>().RelativePath.ShouldBe("two.fzn");
    }

    // ----- Sample 10b: long-form Python pickle ConstrEnum tag -----

    [Fact]
    public void test_interprets_python_pickle_constr_enum_tag()
    {
        // Pattern from spec/unit/regression/bug_empty_enum_extension.mzn and
        // spec/unit/types/non_contig_enum.mzn — uses the !!python/object: form
        // of the ConstrEnum tag rather than the !ConstrEnum shorthand.
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !Result
              solution: !TestCaseSolution
                as:
                - !!python/object:minizinc.types.ConstrEnum
                  argument: z2
                  constructor: z
                - !!python/object:minizinc.types.ConstrEnum
                  argument: x1
                  constructor: x
            """);

        var sol = tc.Expected[0].ShouldBeOfType<ExpectedSolution>().TestCaseSolution;
        var arr = sol.Variables["as"].ShouldBeOfType<ArrayVal>();
        arr.Elements.Count.ShouldBe(2);
        var first = arr.Elements[0].ShouldBeOfType<EnumVal>();
        first.Constructor.ShouldBe("z");
        first.Argument.ShouldBeOfType<StringVal>().Value.ShouldBe("z2");
        var second = arr.Elements[1].ShouldBeOfType<EnumVal>();
        second.Constructor.ShouldBe("x");
        second.Argument.ShouldBeOfType<StringVal>().Value.ShouldBe("x1");
    }

    // ----- Sample 10: nested-array solution (preserves dimensionality) -----

    [Fact]
    public void test_interprets_2d_array_solution_preserving_shape()
    {
        // Pattern from spec/unit/general/test_set_lt_2.mzn (and many others)
        var tc = Interpret("""
            !Test
            solvers: [gecode]
            expected: !Result
              solution: !TestCaseSolution
                grid:
                - [1, 2, 3]
                - [4, 5, 6]
            """);

        var sol = tc.Expected[0].ShouldBeOfType<ExpectedSolution>().TestCaseSolution;
        var arr = sol.Variables["grid"].ShouldBeOfType<ArrayVal>();
        arr.Dimensionality.ShouldBe(2);
        arr.Shape.ShouldBe(new[] { 2, 3 });
        arr.Elements.Count.ShouldBe(6);
        arr.Elements[0].ShouldBeOfType<IntVal>().Value.ShouldBe(1);
        arr.Elements[5].ShouldBeOfType<IntVal>().Value.ShouldBe(6);
    }
}
