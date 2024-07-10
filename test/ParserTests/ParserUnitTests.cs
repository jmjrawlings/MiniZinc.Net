using System.Globalization;
using MiniZinc.Parser.Syntax;

public class ParserUnitTests
{
    [Fact]
    void test_parse_include_item()
    {
        var node = Parser.ParseStatement<IncludeStatement>("include \"xd.mzn\";");
        node.Path.StringValue.Should().Be("xd.mzn");
    }

    [Fact]
    void test_parse_output_item()
    {
        var node = Parser.ParseStatement<OutputStatement>("output [];");
    }

    [Theory]
    [InlineData("enum Letters = {A, B, C};")]
    [InlineData("enum Letters = {A, B, C} ++ {D, E, F};")]
    [InlineData("enum Anon = _(1..10) ++ anon_enum(10);")]
    [InlineData("enum Complex = C(1..10);")]
    void test_parse_enum_item(string mzn)
    {
        var node = Parser.ParseStatement<DeclareStatement>(mzn);
        node.Kind.Should().Be(DeclareKind.Enum);
        node.Body.Should().NotBeNull();
    }

    [Fact]
    void test_parse_constraint()
    {
        var con = Parser.ParseStatement<ConstraintStatement>("constraint a > 2;");
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize abc;")]
    void test_parse_solve(string mzn)
    {
        var node = Parser.ParseStatement<SolveStatement>(mzn);
    }

    [Theory]
    [InlineData("forall(i in 1..3)(true)")]
    void test_parse_gencall_single_name(string mzn)
    {
        var expr = Parser.ParseExpression<GeneratorCallSyntax>(mzn);
        expr.Name.ToString().Should().Be("forall");
        expr.Generators.Should()
            .SatisfyRespectively(gen =>
            {
                gen.Names.Should()
                    .SatisfyRespectively(name =>
                    {
                        name.ToString().Should().Be("i");
                    });
            });
    }

    [Theory]
    [InlineData("forall(i,j,k in 1..3)(true)")]
    void test_parse_gencall_multiple_names(string mzn)
    {
        var expr = Parser.ParseExpression<GeneratorCallSyntax>(mzn);
        expr.Name.ToString().Should().Be("forall");
        expr.Generators.Should()
            .SatisfyRespectively(gen =>
            {
                gen.Names.Select(x => x.ToString()).Should().Equal("i", "j", "k");
            });
    }

    [Theory]
    [InlineData("forall (i, j, k, l in -3..3 where i <= j /\\ k <= l)(true)")]
    void test_parse_gencall_multiple_names_multiple_filters(string mzn)
    {
        var expr = Parser.ParseExpression<GeneratorCallSyntax>(mzn);
        expr.Name.ToString().Should().Be("forall");
        expr.Generators.Should()
            .SatisfyRespectively(gen =>
            {
                gen.Names.Select(x => x.ToString()).Should().Equal("i", "j", "k", "l");
            });
    }

    [Fact]
    void test_parser_gencall_2()
    {
        var mzn = "sum (i in class where i >= s) (class_sizes[i])";
        var call = Parser.ParseExpression<GeneratorCallSyntax>(mzn);
        call.Name.Should().Be("sum");
        call.Expr.Should().BeOfType<ArrayAccessSyntax>();
        call.Generators.Should().HaveCount(1);
        var gen = call.Generators[0];
        gen.Names.Should().HaveCount(1);
        var name = gen.Names[0].Name;
        name.Should().Be("i");
    }

    [Theory]
    [InlineData("a[Fst[i] + j * (i + 1)]")]
    void test_array_access(string mzn)
    {
        var node = Parser.ParseExpression<ArrayAccessSyntax>(mzn);
    }

    [Theory]
    [InlineData("[ 1: 1, 2: 2, 3: 3, 4: 4, 5: 5]")]
    [InlineData("[ A: 0, B: 3, C: 5]")]
    [InlineData("[ (1,2): 1, (1,3): 2, (2,2): 3, (2,3): 4]")]
    [InlineData("[ 1: 1, 4: 2, 5: 3, 3: 4, 2: 5]")]
    [InlineData("[ 1: 1, 2, 3, 4]")]
    void test_indexed_array_1d(string mzn)
    {
        var expr = Parser.ParseExpression<Array1dSyntax>(mzn);
    }

    [Fact]
    void test_array2d_column_indexed()
    {
        var mzn = "[| A: B: C:\n | 0, 0, 0\n | 1, 1, 1\n | 2, 2, 2 |];";
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
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
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
        arr.RowIndexed.Should().BeTrue();
        arr.ColIndexed.Should().BeFalse();
        arr.Elements.Should().HaveCount(9);
        arr.Indices.Should().HaveCount(3);
        arr.Indices[0].ToString().Should().Be("A");
        arr.Indices[1].ToString().Should().Be("B");
        arr.Indices[2].ToString().Should().Be("C");
    }

    [Fact]
    void test_array2d_opt()
    {
        var mzn = "[|<>, 5,|5, 5,||]";
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
        arr.I.Should().Be(2);
        arr.J.Should().Be(2);
        arr.Elements.Count.Should().Be(4);
    }

    [Fact]
    void test_array2d_dual_indexed()
    {
        var mzn = "[| A: B: C:\n | A: 0, 0, 0\n | B: 1, 1, 1\n | C: 2, 2, 2 |]";
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
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
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
        arr.RowIndexed.Should().BeFalse();
        arr.ColIndexed.Should().BeFalse();
        arr.Elements.Should().HaveCount(24);
    }

    [Fact]
    void test_array2d_call()
    {
        var mzn = """
                [| 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, _, _, _, _, _, _, _, 0, _, 0, 0
                 | 0, _, _, _, _, _, _, _, _, 0, _, 0
                 | 0, _, _, _, _, _, _, _, _, _, _, 0
                 | 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
                 |]
            )
            """;
        var arr = Parser.ParseExpression<Array2dSyntax>(mzn);
        arr.I.Should().Be(12);
        arr.J.Should().Be(12);
        arr.Elements.Count.Should().Be(144);
    }

    [Fact]
    void test_expr_type_inst()
    {
        var mzn = "record(1..1:x): a";
        var node = Parser.ParseStatement<DeclareStatement>(mzn);
        var record = node.Type as RecordTypeSyntax;
        var field = record!.Fields[0];
        field.Identifier.ToString().Should().Be("x");
        var ti = field.Type as ExprType;
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
        var let = Parser.ParseExpression<LetSyntax>(mzn);
        let.Locals.Should().HaveCount(2);
        let.Body.Should().BeOfType<IdentifierSyntax>();
        let.Body.ToString().Should().Be("res");
    }

    [Fact]
    void test_partial_range_ti()
    {
        var mzn = "0..: xd;";
        var node = Parser.ParseStatement<DeclareStatement>(mzn);
        var type = node.Type as ExprType;
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
        var expr = Parser.ParseExpression<ComprehensionSyntax>(mzn);
    }

    [Fact]
    void test_set_of_ti()
    {
        var mzn = "set of var int: xd";
        var node = Parser.ParseStatement<DeclareStatement>(mzn);
        node.Identifier.ToString().Should().Be("xd");
    }

    [Fact]
    void test_postfix_range_operator()
    {
        var mzn = "var 0..: xd";
        var node = Parser.ParseStatement<DeclareStatement>(mzn);
        node.Identifier.ToString().Should().Be("xd");
        var type = (ExprType)node.Type;
        type.Var.Should().BeTrue();
        var range = (RangeLiteralSyntax)type.Expr;
        range.Lower.Should().BeOfType<IntLiteralSyntax>();
        range.Upper.Should().BeNull();
        var lo = (IntLiteralSyntax)range.Lower!;
        lo.Value.Should().Be(0);
    }

    [Fact]
    void test_array3d_literal()
    {
        var mzn = "[| |1,1|1,1|, |2,2|2,2|, |3,3|3,3| |]";
        var arr = Parser.ParseExpression<Array3dSyntax>(mzn);
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
        var arr = Parser.ParseExpression<Array3dSyntax>(mzn);
        arr.I.Should().Be(0);
        arr.J.Should().Be(0);
        arr.K.Should().Be(0);
        arr.Elements.Should().BeEmpty();
    }

    [Theory]
    [InlineData("annotation xd")]
    [InlineData("annotation something(int: x)")]
    void test_annotation_declaration(string mzn)
    {
        var ann = Parser.ParseStatement<DeclareStatement>(mzn);
        ann.Type.Kind.Should().Be(TypeKind.Annotation);
    }

    [Theory]
    // [InlineData("1+2", 3)]
    // [InlineData("1+2*4", 9)]
    // [InlineData("(1+2)*4", 12)]
    [InlineData("2*4-1", 7)]
    void test_operator_precedence(string mzn, int expected)
    {
        var expr = Parser.ParseExpression<ExpressionSyntax>(mzn);
        int eval(SyntaxNode expr)
        {
            if (expr is IntLiteralSyntax i)
                return i.Value;

            if (expr is not BinaryOperatorSyntax binop)
                throw new Exception();

            var left = eval(binop.Left);
            var right = eval(binop.Right);
            switch (binop.Operator)
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

    [Fact]
    void test_float_range()
    {
        var mzn = "0.01..1.123;";
        var expr = Parser.ParseExpression<RangeLiteralSyntax>(mzn);
        expr.Lower.Should().BeOfType<FloatLiteralSyntax>();
        expr.Upper.Should().BeOfType<FloatLiteralSyntax>();
    }

    [Fact]
    void test_parse_operator_same_precedence()
    {
        var mzn = "a diff (b union c)";
        var expr = Parser.ParseExpression<BinaryOperatorSyntax>(mzn);
        var ozn = expr.Write();
        ozn.Should().Be("a diff (b union c)");
    }

    [Fact]
    void test_parse_unary_prec()
    {
        var mzn = @"not(A -> B) -> not(C -> D)";
        var expr = Parser.ParseExpression<BinaryOperatorSyntax>(mzn);
        expr.Left.Should().BeOfType<UnaryOperatorSyntax>();
        expr.Right.Should().BeOfType<UnaryOperatorSyntax>();
    }

    [Fact]
    void test_parse_precedence_left_assoc_equal()
    {
        var mzn = "1 + 2 - 3";
        var binop = Parser.ParseExpression<BinaryOperatorSyntax>(mzn);
        binop.Operator.Should().Be(Operator.Subtract);
        var oz = binop.Write();
        oz.Should().Be(mzn);
        var b = 2;
    }

    [Fact]
    void test_parse_precedence_left_assoc_descending()
    {
        var mzn = "a - b >= c";
        var binop = Parser.ParseExpression<BinaryOperatorSyntax>(mzn);
        binop.Operator.Should().Be(Operator.GreaterThanEqual);
        var oz = binop.Write();
        oz.Should().Be(mzn);
    }

    [Fact]
    void test_write_precedence_left_assoc_brackets()
    {
        var mzn = "(Formula[1] > 0) == assignment[1]";
        var binop = Parser.ParseExpression<BinaryOperatorSyntax>(mzn);
        binop.Operator.Should().Be(Operator.Equal);
        var oz = binop.Write();
        oz.Should().Be(mzn);
    }

    [Fact]
    void test_parse_right_assoc()
    {
        var mzn = "var MyTuple ++ var MyTuple: tuptup ::output = tup ++ tup;";
        var binop = Parser.ParseStatement<DeclareStatement>(mzn);
        var oz = binop.Write();
        oz.Should().BeEquivalentTo(mzn);
    }

    [Fact]
    void test_parse_union_type_arg()
    {
        var mzn = "var ..-1 union {1,3}";
        var parser = new Parser(mzn);
        var ok = parser.ParseType(out var type);
        ok.Should().BeTrue();
        var a = 2;
    }

    [Theory]
    [InlineData("-2.8421709430404e-14")]
    [InlineData("2e10")]
    void test_parse_float(string f)
    {
        var mzn = f;
        var node = Parser.ParseExpression<FloatLiteralSyntax>(mzn);
        var a = 2;
    }

    [Fact]
    void test_parse_generic_func()
    {
        var mzn = "$T: foo(tuple($T): x) = x.1;";
        var parser = new Parser(mzn);
        var ok = parser.ParseDeclareStatement(out var node);
        ok.Should().BeTrue();
    }

    ModelSyntax ParseString(string mzn)
    {
        var result = Parser.ParseModelString(mzn, out var model);
        result.ErrorTrace.Should().BeNull();
        return model;
    }
}
