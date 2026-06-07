namespace MiniZinc.TestSpec.Tests;

/// <summary>
/// Low-level scanner tests. Each test drives a raw YAML payload through
/// <see cref="YamlScanner"/> and asserts the shape of the resulting
/// <see cref="YamlNode"/> tree.
/// </summary>
public class YamlScannerTests
{
    [Fact]
    public void scans_simple_block_mapping()
    {
        const string yaml = """
            foo: bar
            count: 42
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        docs.Count.ShouldBe(1);
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        map.Entries.Count.ShouldBe(2);
        map.Entries[0].Key.Value.ShouldBe("foo");
        map.Entries[0].Value.ShouldBeOfType<YamlScalar>().Value.ShouldBe("bar");
        map.Entries[1].Key.Value.ShouldBe("count");
        map.Entries[1].Value.ShouldBeOfType<YamlScalar>().Value.ShouldBe("42");
    }

    [Fact]
    public void scans_inline_flow_sequence()
    {
        const string yaml = "solvers: [gecode, chuffed]";
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var seq = map.Entries[0].Value.ShouldBeOfType<YamlSequence>();
        seq.Items.Count.ShouldBe(2);
        seq.Items[0].ShouldBeOfType<YamlScalar>().Value.ShouldBe("gecode");
        seq.Items[1].ShouldBeOfType<YamlScalar>().Value.ShouldBe("chuffed");
    }

    [Fact]
    public void scans_tagged_block_mapping()
    {
        const string yaml = """
            !Test
            solvers: [gecode]
            expected: !FlatZinc aggregation.fzn
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        map.Tag.ShouldBe("!Test");
        map.Entries.Count.ShouldBe(2);
        var expected = map.Entries[1].Value.ShouldBeOfType<YamlScalar>();
        expected.Tag.ShouldBe("!FlatZinc");
        expected.Value.ShouldBe("aggregation.fzn");
    }

    [Fact]
    public void scans_block_literal_scalar()
    {
        const string yaml = """
            _output_item: |
              line one
              line two
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var s = map.Entries[0].Value.ShouldBeOfType<YamlScalar>();
        s.Style.ShouldBe(ScalarStyle.Literal);
        s.Value.ShouldContain("line one");
        s.Value.ShouldContain("line two");
    }

    [Fact]
    public void scans_double_doc_with_doc_separator()
    {
        const string yaml = """
            --- !Test
            solvers: [gecode]
            --- !Test
            solvers: [chuffed]
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        docs.Count.ShouldBe(2);
        docs[0].Tag.ShouldBe("!Test");
        docs[1].Tag.ShouldBe("!Test");
    }

    [Fact]
    public void scans_flow_style_set()
    {
        // `dset: !!set {"Fri", "Sat", "Sun"}` — pattern from spec/unit/json/coerce_enum_str.mzn
        const string yaml = """dset: !!set {"Fri", "Sat", "Sun"}""";
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var setNode = map.Entries[0].Value.ShouldBeOfType<YamlMapping>();
        setNode.Tag.ShouldBe("!!set");
        setNode.Entries.Count.ShouldBe(3);
        setNode.Entries[0].Key.Value.ShouldBe("Fri");
    }

    [Fact]
    public void scans_compact_block_sequence_under_key()
    {
        // The block sequence is at the same indent as its parent key — YAML "compact" form.
        const string yaml = """
            expected:
            - !Result
              status: SATISFIED
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var seq = map.Entries[0].Value.ShouldBeOfType<YamlSequence>();
        seq.Items.Count.ShouldBe(1);
        var result = seq.Items[0].ShouldBeOfType<YamlMapping>();
        result.Tag.ShouldBe("!Result");
    }

    [Fact]
    public void scans_nested_compact_block_sequences()
    {
        // Pattern from spec/unit/types/common_struct_bottom.mzn — nested array literals.
        const string yaml = """
            x:
            - - - 1
            - - 2
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var outer = map.Entries[0].Value.ShouldBeOfType<YamlSequence>();
        outer.Items.Count.ShouldBe(2);
        var nested0 = outer.Items[0].ShouldBeOfType<YamlSequence>();
        nested0.Items[0].ShouldBeOfType<YamlSequence>().Items[0]
            .ShouldBeOfType<YamlScalar>().Value.ShouldBe("1");
        var nested1 = outer.Items[1].ShouldBeOfType<YamlSequence>();
        nested1.Items[0].ShouldBeOfType<YamlScalar>().Value.ShouldBe("2");
    }

    [Fact]
    public void scans_quoted_strings()
    {
        const string yaml = """
            single: 'hello'
            double: "world"
            empty: ''
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        var s = map.Entries[0].Value.ShouldBeOfType<YamlScalar>();
        s.Style.ShouldBe(ScalarStyle.SingleQuoted);
        s.Value.ShouldBe("hello");
        var d = map.Entries[1].Value.ShouldBeOfType<YamlScalar>();
        d.Style.ShouldBe(ScalarStyle.DoubleQuoted);
        d.Value.ShouldBe("world");
        map.Entries[2].Value.ShouldBeOfType<YamlScalar>().Value.ShouldBe("");
    }

    [Fact]
    public void duplicate_keys_are_last_wins()
    {
        // Real pattern from spec/unit/globals/cumulative/github_589.mzn —
        // upstream YAML occasionally has an empty placeholder followed by the
        // real value. PyYAML (used by the libminizinc harness) tolerates this
        // with last-wins semantics, so we mirror that rather than erroring.
        const string yaml = """
            solution: !Placeholder
            status: ALL_SOLUTIONS
            solution: !SolutionSet
            """;
        var docs = new YamlScanner(yaml).ParseStream();
        var map = docs[0].ShouldBeOfType<YamlMapping>();
        // The duplicate key collapses to a single entry, keeping its original
        // position but taking the later value/tag.
        map.Entries.Count.ShouldBe(2);
        map.Entries[0].Key.Value.ShouldBe("solution");
        map.Entries[0].Value.ShouldBeOfType<YamlScalar>().Tag.ShouldBe("!SolutionSet");
        map.Entries[1].Key.Value.ShouldBe("status");
    }
}
