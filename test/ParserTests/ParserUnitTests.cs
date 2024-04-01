using MiniZinc.Parser.Ast;

public static class ParserExtensions
{
    public static void Check(this Parser p)
    {
        if (p.Err is { } err)
            Assert.Fail(err);
    }
}

public class ParserUnitTests
{
    Parser Parse(string mzn)
    {
        var lexer = Lexer.LexString(mzn);
        var parser = new Parser(lexer);

        parser.Step();
        return parser;
    }

    [Fact]
    void test_namespace()
    {
        var ns = new NameSpace<int>();
        ns.Push("a", 1);
        ns.Push("a", 2);
        ns.Push("a", 3);
        ns["a"].Should().Be(3);
        ns.Pop();
        ns["a"].Should().Be(2);
        ns.Pop().Value.Should().Be(2);
        ns["a"].Should().Be(1);
        ns.Pop();
        ns.ContainsKey("a").Should().BeFalse();
    }

    [Fact]
    void test_parse_include_item()
    {
        var parser = Parse("include \"xd.mzn\";");
        var model = new Model();
        parser.ParseIncludeItem(model);
        model.Includes.Should().HaveCount(1);
    }

    [Fact]
    void test_parse_output_item()
    {
        var parser = Parse("output [];");
        var model = new Model();
        parser.ParseOutputItem(model);
        model.Outputs.Should().HaveCount(1);
    }

    [Theory]
    [InlineData("enum Letters = {A, B, C};")]
    [InlineData("enum Letters = {A, B, C} ++ {D, E, F};")]
    [InlineData("enum Anon = _(1..10) ++ anon_enum(10);")]
    [InlineData("enum Complex = C(1..10);")]
    void test_parse_enum_item(string mzn)
    {
        var parser = Parse(mzn);
        var model = new Model();
        parser.ParseEnumItem(out var @enum).Should().BeTrue();
    }

    [Fact]
    void test_parse_constraint()
    {
        var parser = Parse("constraint a > 2;");
        parser.ParseConstraintItem(out var con);
        parser.Check();
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize abc;")]
    void test_parse_solve(string mzn)
    {
        var p = Parse(mzn);
        var model = new Model();
        p.ParseSolveItem(model);
        model.SolveItems.Should().NotBeNull();
        p.Check();
    }

    [Theory]
    [InlineData("forall(i in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i,j in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i in 1..3, j in 1..3 where i > j)(xd[i] > 0)")]
    void test_parse_gencall(string mzn)
    {
        var p = Parse(mzn);
        p.ParseIdentExpr(out var expr);
        expr.Should().BeOfType<GenCallExpr>();
    }

    [Theory]
    [InlineData("a[Fst[i] + j * (i + 1)]")]
    void test_array_access(string mzn)
    {
        var p = Parse(mzn);
        p.ParseExpr(out var expr);
        p.Check();
        expr.Should().BeOfType<ArrayAccessExpr>();
    }

    [Theory]
    [InlineData("[ 1: 1, 2: 2, 3: 3, 4: 4, 5: 5]")]
    [InlineData("[ A: 0, B: 3, C: 5]")]
    [InlineData("[ (1,2): 1, (1,3): 2, (2,2): 3, (2,3): 4]")]
    [InlineData("[ 1: 1, 4: 2, 5: 3, 3: 4, 2: 5]")]
    [InlineData("[ 1: 1, 2, 3, 4]")]
    void test_indexed_array_1d(string mzn)
    {
        var parser = Parse(mzn);
        parser.ParseBracketExpr(out var expr);
        expr.Should().BeOfType<Array1DLit>();
        var arr = (Array1DLit)expr;

        parser.Check();
    }

    [Fact]
    void test_array2d_column_indexed()
    {
        var mzn = "[| A: B: C:\n | 0, 0, 0\n | 1, 1, 1\n | 2, 2, 2 |];";
        var parser = Parse(mzn);
        parser.ParseBracketExpr(out var expr);
        expr.Should().BeOfType<Array2DLit>();
        var arr = (Array2DLit)expr;
        arr.RowIndexed.Should().BeFalse();
        arr.ColIndexed.Should().BeTrue();
        arr.Elements.Should().HaveCount(9);
        arr.Indices.Should().HaveCount(3);
        arr.Indices[0].ToString().Should().Be("A");
        arr.Indices[1].ToString().Should().Be("B");
        arr.Indices[2].ToString().Should().Be("C");
    }

    [Fact]
    void test_array2d_row_indexed()
    {
        var mzn = "[| A: 0, 0, 0\n | B: 1, 1, 1\n | C: 2, 2, 2 |];";
        var parser = Parse(mzn);
        parser.ParseBracketExpr(out var expr);
        expr.Should().BeOfType<Array2DLit>();
        var arr = (Array2DLit)expr;
        arr.RowIndexed.Should().BeTrue();
        arr.ColIndexed.Should().BeFalse();
        arr.Elements.Should().HaveCount(9);
        arr.Indices.Should().HaveCount(3);
        arr.Indices[0].ToString().Should().Be("A");
        arr.Indices[1].ToString().Should().Be("B");
        arr.Indices[2].ToString().Should().Be("C");
    }

    [Fact]
    void test_array2d_dual_indexed()
    {
        var mzn = "[| A: B: C:\n | A: 0, 0, 0\n | B: 1, 1, 1\n | C: 2, 2, 2 |]";
        var parser = Parse(mzn);
        parser.ParseBracketExpr(out var expr);
        expr.Should().BeOfType<Array2DLit>();
        var arr = (Array2DLit)expr;
        arr.RowIndexed.Should().BeTrue();
        arr.ColIndexed.Should().BeTrue();
        arr.Elements.Should().HaveCount(9);
        arr.Indices.Should().HaveCount(6);
        arr.Indices[0].ToString().Should().Be("A");
        arr.Indices[1].ToString().Should().Be("B");
        arr.Indices[2].ToString().Should().Be("C");
        arr.Indices[3].ToString().Should().Be("A");
        arr.Indices[4].ToString().Should().Be("B");
        arr.Indices[5].ToString().Should().Be("C");
    }

    [Fact]
    void test_array2d_no_index()
    {
        var mzn = "[| 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 | 0, _, _, _, _, _, _, _, _, _, _, 0|]";
        var parser = Parse(mzn);
        parser.ParseBracketExpr(out var expr);
        expr.Should().BeOfType<Array2DLit>();
        var arr = (Array2DLit)expr;
        arr.RowIndexed.Should().BeFalse();
        arr.ColIndexed.Should().BeFalse();
        arr.Elements.Should().HaveCount(24);
    }

    [Fact]
    void test_expr_type_inst()
    {
        var mzn = "record(1..1:x): a";
        var parser = Parse(mzn);
        parser.ParseDeclareOrAssignItem(out var var, out var assign);
        var.Should().NotBeNull();
        var!.Type.Should().BeOfType<RecordTypeInst>();
        var rec = (RecordTypeInst)var.Type;
        rec.Fields[0].Name.Should().Be("x");
        var ti = rec.Fields[0].Value as ExprTypeInst;
        var rng = (RangeExpr)ti.Expr;
        ((IntLit)rng.Lower).Value.Should().Be(1);
        ((IntLit)rng.Upper).Value.Should().Be(1);
    }
}
