using MiniZinc.Parser.Ast;

public static class ParserExtensions
{
    public static void Check(this Parser p)
    {
        if (p.ErrorString is { } err)
            Assert.Fail(err);
    }
}

public class ParserUnitTests
{
    SyntaxTree ParseText(string mzn)
    {
        var parser = new Parser(mzn);
        if (!parser.ParseModel(out var tree))
            Assert.Fail(parser.ErrorString ?? "");
        return tree;
    }

    T ParseExpr<T>(string mzn)
        where T : SyntaxNode
    {
        var parser = new Parser(mzn);
        var result = parser.ParseExpr(out var expr);
        result.Should().BeTrue();
        expr.Should().BeOfType<T>();
        return (T)expr;
    }

    T ParseNode<T>(string mzn)
    {
        var tree = ParseText(mzn);
        var nodes = tree.Nodes;
        nodes.AddRange(tree.Includes);
        nodes.AddRange(tree.Constraints);
        nodes.AddRange(tree.SolveItems);
        nodes.AddRange(tree.Aliases);
        nodes.AddRange(tree.Outputs);
        nodes.AddRange(tree.Enums);

        var node = nodes[0];
        if (node is not T t)
        {
            Assert.Fail($"Parsed node {node} was not of expected type {typeof(T)}");
            return default;
        }

        return t;
    }

    [Fact]
    void test_namespace()
    {
        var ns = new NameSpace<int>();
        ns.Add("a", 1);
        ns.Add("a", 2);
        ns.Add("a", 3);
        ns["a"].Should().Be(3);
        // ns.Pop();
        // ns["a"].Should().Be(2);
        // ns.Pop().Value.Should().Be(2);
        // ns["a"].Should().Be(1);
        // ns.Pop();
        // ns.ContainsKey("a").Should
        // ().BeFalse();
    }

    [Fact]
    void test_parse_include_item()
    {
        var node = ParseNode<IncludeSyntax>("include \"xd.mzn\";");
        node.Path.StringValue.Should().Be("xd.mzn");
    }

    [Fact]
    void test_parse_output_item()
    {
        var node = ParseNode<OutputSyntax>("output [];");
    }

    [Theory]
    [InlineData("enum Letters = {A, B, C};")]
    [InlineData("enum Letters = {A, B, C} ++ {D, E, F};")]
    [InlineData("enum Anon = _(1..10) ++ anon_enum(10);")]
    [InlineData("enum Complex = C(1..10);")]
    void test_parse_enum_item(string mzn)
    {
        var node = ParseNode<EnumDeclarationSyntax>(mzn);
    }

    [Fact]
    void test_parse_constraint()
    {
        var con = ParseNode<ConstraintSyntax>("constraint a > 2;");
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize abc;")]
    void test_parse_solve(string mzn)
    {
        var node = ParseNode<SolveSyntax>(mzn);
    }

    [Theory]
    [InlineData("forall(i in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i,j in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i in 1..3, j in 1..3 where i > j)(xd[i] > 0)")]
    void test_parse_gencall(string mzn)
    {
        var expr = ParseExpr<GeneratorCallSyntax>(mzn);
    }

    [Theory]
    [InlineData("a[Fst[i] + j * (i + 1)]")]
    void test_array_access(string mzn)
    {
        var node = ParseExpr<ArrayAccessExpr>(mzn);
    }

    [Theory]
    [InlineData("[ 1: 1, 2: 2, 3: 3, 4: 4, 5: 5]")]
    [InlineData("[ A: 0, B: 3, C: 5]")]
    [InlineData("[ (1,2): 1, (1,3): 2, (2,2): 3, (2,3): 4]")]
    [InlineData("[ 1: 1, 4: 2, 5: 3, 3: 4, 2: 5]")]
    [InlineData("[ 1: 1, 2, 3, 4]")]
    void test_indexed_array_1d(string mzn)
    {
        var expr = ParseExpr<Array1DExpr>(mzn);
    }

    [Fact]
    void test_array2d_column_indexed()
    {
        var mzn = "[| A: B: C:\n | 0, 0, 0\n | 1, 1, 1\n | 2, 2, 2 |];";
        var arr = ParseExpr<Array2dExpr>(mzn);
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
        var arr = ParseExpr<Array2dExpr>(mzn);
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
        var arr = ParseExpr<Array2dExpr>(mzn);
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
        var arr = ParseExpr<Array2dExpr>(mzn);
        arr.RowIndexed.Should().BeFalse();
        arr.ColIndexed.Should().BeFalse();
        arr.Elements.Should().HaveCount(24);
    }

    [Fact]
    void test_expr_type_inst()
    {
        var mzn = "record(1..1:x): a";
        var node = ParseNode<VariableDeclarationSyntax>(mzn);
        var record = node.Type as RecordTypeInstSyntax;
        var (name, type) = record!.Items[0];
        name.ToString().Should().Be("x");
        var ti = type as ExprTypeInst;
        var rng = (RangeLiteralSyntax)ti!.Expr;
        ((IntLiteralSyntax)rng.Lower!).Value.Should().Be(1);
        ((IntLiteralSyntax)rng.Upper!).Value.Should().Be(1);
    }

    [Fact]
    void test_parse_let_xd()
    {
        var mzn = """
            let {
                array[1..1] of var bool: res;
                constraint res[1];
            } in res;
            """;
        var let = ParseExpr<LetSyntax>(mzn);
        let.Locals.Should().HaveCount(2);
        let.Body.Should().BeOfType<IdentifierSyntax>();
        let.Body.ToString().Should().Be("res");
    }

    [Fact]
    void test_partial_range_ti()
    {
        var mzn = "0..: xd;";
        var node = ParseNode<VariableDeclarationSyntax>(mzn);
        var type = node.Type as ExprTypeInst;
        var expr = type!.Expr;
        expr.Should().BeOfType<RangeLiteralSyntax>();
        var rng = (RangeLiteralSyntax)expr;
        rng.Upper.Should().BeNull();
        // rng.Lower.Should().Be(new IntLiteralSyntax(0));
    }

    [Fact]
    void test_record_comp()
    {
        var mzn = """
            [
              i: (a: some_map[i], b: some_map[i] mod 2 = 0) | i in Some
            ]
            """;
        var expr = ParseExpr<ComprehensionSyntax>(mzn);
    }

    [Fact]
    void test_set_of_ti()
    {
        var mzn = "set of var int: xd";
        var node = ParseNode<VariableDeclarationSyntax>(mzn);
        node.Name.ToString().Should().Be("xd");
    }

    [Fact]
    void test_postfix_range_operator()
    {
        var mzn = "var 0..: xd";
        var node = ParseNode<VariableDeclarationSyntax>(mzn);
        node.Name.StringValue.Should().Be("xd");
        var type = node.Type as ExprTypeInst;
        // type.Var.Should().BeTrue();
        // var range = (RangeLiteralSyntax)type.Expr;
        // range.Lower.Should().Be(new IntLiteralSyntax(0));
        // range.Upper.Should().BeNull();
    }

    [Fact]
    void test_array3d_literal()
    {
        var mzn = "[| |1,1|1,1|, |2,2|2,2|, |3,3|3,3| |]";
        var arr = ParseExpr<Array3dExpr>(mzn);
        arr.I.Should().Be(3);
        arr.J.Should().Be(2);
        arr.K.Should().Be(2);
        arr.Elements.Should().HaveCount(12);
        var numbers = arr.Elements.Select(x => (int)(IntLiteralSyntax)x);
        numbers.Should().Equal(1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3);
    }

    [Fact]
    void test_array3d_empty()
    {
        var mzn = "[| || |]";
        var arr = ParseExpr<Array3dExpr>(mzn);
        arr.I.Should().Be(0);
        arr.J.Should().Be(0);
        arr.K.Should().Be(0);
        arr.Elements.Should().BeEmpty();
    }

    [Theory]
    // [InlineData("1+2", 3)]
    // [InlineData("1+2*4", 9)]
    // [InlineData("(1+2)*4", 12)]
    [InlineData("2*4-1", 7)]
    void test_operator_precedence(string mzn, int expected)
    {
        var parser = new Parser(mzn);
        parser.ParseExpr(out var expr);
        int eval(SyntaxNode expr)
        {
            if (expr is IntLiteralSyntax i)
                return i.Value;

            if (expr is not BinaryOperatorSyntax binop)
                throw new Exception();

            var left = eval(binop.Left);
            var right = eval(binop.Right);
            switch (binop.Op)
            {
                case Operator.Add:
                    return left + right;
                case Operator.Subtract:
                    return left - right;
                case Operator.Multiply:
                    return left * right;
                case Operator.Div:
                    return left / right;
                default:
                    throw new Exception();
            }
        }

        var result = eval(expr);
        result.Should().Be(expected);
    }
}
