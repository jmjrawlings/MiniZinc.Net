using MiniZinc.Parser;
using Shouldly;
using Xunit;
using static MiniZinc.Parser.TokenKind;

public class ParserUnitTests
{
    [Fact]
    public void test_parse_include_item()
    {
        var node = Parser.ParseItem<IncludeItem>("include \"xd.mzn\";");
        node.Path.StringValue.ShouldBe("xd.mzn");
    }

    [Fact]
    public void test_parse_output_item()
    {
        var node = Parser.ParseItem<OutputItem>("output [];");
    }

    [Theory]
    [InlineData("enum Letters = {A, B, C};")]
    [InlineData("enum Letters = {A, B, C} ++ {D, E, F};")]
    [InlineData("enum Anon = _(1..10) ++ anon_enum(10);")]
    [InlineData("enum Complex = C(1..10);")]
    public void test_parse_enum_item(string mzn)
    {
        var node = Parser.ParseItem<DeclareItem>(mzn);
        node.Kind.ShouldBe(DeclareKind.DECLARE_ENUM);
        node.Expr.ShouldNotBeNull();
    }

    [Fact]
    public void test_parse_constraint()
    {
        var con = Parser.ParseItem<ConstraintItem>("constraint a > 2;");
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize abc;")]
    public void test_parse_solve(string mzn)
    {
        var node = Parser.ParseItem<SolveItem>(mzn);
    }

    [Theory]
    [InlineData("forall(i in 1..3)(true)")]
    public void test_parse_gencall_single_name(string mzn)
    {
        var expr = Parser.ParseExpression<GenCallExpr>(mzn);
        expr.Name.ToString().ShouldBe("forall");
        // expr.Generators.shouldsa(gen =>
        // {
        //     gen.Should()
        //         .BeOfType<GenYieldExpr>()
        //         .Which.Ids.Should()
        //         .SatisfyRespectively(id => id.StringValue.ShouldBe("i"));
        // });
    }

    [Theory]
    [InlineData("forall(i,j,k in 1..3)(true)")]
    public void test_parse_gencall_multiple_names(string mzn)
    {
        var expr = Parser.ParseExpression<GenCallExpr>(mzn);
        expr.Name.ToString().ShouldBe("forall");
        var gen = expr.Generators[0].ShouldBeOfType<GenYieldExpr>();
        gen.Ids.Select(t => t.StringValue).ShouldBe(["i", "j", "k"]);
    }

    [Theory]
    [InlineData("forall (i, j, k, l in -3..3 where i <= j /\\ k <= l)(true)")]
    public void test_parse_gencall_multiple_names_multiple_filters(string mzn)
    {
        var expr = Parser.ParseExpression<GenCallExpr>(mzn);
        expr.Name.ToString().ShouldBe("forall");
        // expr.Generators.Should()
        //     .SatisfyRespectively(gen =>
        //     {
        //         gen.Names.Select(x => x.ToString()).ShouldEqual("i", "j", "k", "l");
        //     });
    }

    [Fact]
    public void test_parse_gencall_yield_with_filter()
    {
        var mzn = "sum (i in class where i >= s) (class_sizes[i])";
        var call = Parser.ParseExpression<GenCallExpr>(mzn);
        call.Name.StringValue.ShouldBe("sum");
        call.Expr.ShouldBeOfType<ArrayAccessExpr>();
        var gen = call.Generators[0].ShouldBeOfType<GenYieldExpr>();
        gen.Ids[0].StringValue.ShouldBe("i");
        gen.Source.Start.StringValue.ShouldBe("class");
        gen.Where!.ToString().ShouldBe("i>=s");
    }

    [Theory]
    [InlineData("a[Fst[i] + j * (i + 1)]")]
    public void test_array_access(string mzn)
    {
        var node = Parser.ParseExpression<ArrayAccessExpr>(mzn);
    }

    [Theory]
    [InlineData("[ 1: 1, 2: 2, 3: 3, 4: 4, 5: 5]")]
    [InlineData("[ A: 0, B: 3, C: 5]")]
    [InlineData("[ (1,2): 1, (1,3): 2, (2,2): 3, (2,3): 4]")]
    [InlineData("[ 1: 1, 4: 2, 5: 3, 3: 4, 2: 5]")]
    [InlineData("[ 1: 1, 2, 3, 4]")]
    public void test_indexed_array_1d(string mzn)
    {
        var expr = Parser.ParseExpression<Array1dExpr>(mzn);
    }

    [Fact]
    public void test_array2d_column_indexed()
    {
        var mzn = "[| A: B: C:\n | 0, 0, 0\n | 1, 1, 1\n | 2, 2, 2 |];";
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.RowIndexed.ShouldBeFalse();
        arr.ColIndexed.ShouldBeTrue();
        arr.Elements.Count.ShouldBe(9);
        arr.Indices.Count.ShouldBe(3);
        arr.Indices[0].ToString().ShouldBe("A");
        arr.Indices[1].ToString().ShouldBe("B");
        arr.Indices[2].ToString().ShouldBe("C");
    }

    [Fact]
    public void test_array2d_row_indexed()
    {
        var mzn = "[| A: 0, 0, 0\n | B: 1, 1, 1\n | C: 2, 2, 2 |];";
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.RowIndexed.ShouldBeTrue();
        arr.ColIndexed.ShouldBeFalse();
        arr.Elements.Count.ShouldBe(9);
        arr.Indices.Count.ShouldBe(3);
        arr.Indices[0].ToString().ShouldBe("A");
        arr.Indices[1].ToString().ShouldBe("B");
        arr.Indices[2].ToString().ShouldBe("C");
    }

    [Fact]
    public void test_array2d_opt()
    {
        var mzn = "[|<>, 5,|5, 5,||]";
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.I.ShouldBe(2);
        arr.J.ShouldBe(2);
        arr.Elements.Count.ShouldBe(4);
    }

    [Fact]
    public void test_array2d_dual_indexed()
    {
        var mzn = "[| A: B: C:\n | A: 0, 0, 0\n | B: 1, 1, 1\n | C: 2, 2, 2 |]";
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.RowIndexed.ShouldBeTrue();
        arr.ColIndexed.ShouldBeTrue();
        arr.Elements.Count.ShouldBe(9);
        arr.Indices.Count.ShouldBe(6);
        arr.Indices[0].ToString().ShouldBe("A");
        arr.Indices[1].ToString().ShouldBe("B");
        arr.Indices[2].ToString().ShouldBe("C");
        arr.Indices[3].ToString().ShouldBe("A");
        arr.Indices[4].ToString().ShouldBe("B");
        arr.Indices[5].ToString().ShouldBe("C");
    }

    [Fact]
    public void test_array2d_no_index()
    {
        var mzn = "[| 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 | 0, _, _, _, _, _, _, _, _, _, _, 0|]";
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.RowIndexed.ShouldBeFalse();
        arr.ColIndexed.ShouldBeFalse();
        arr.Elements.Count.ShouldBe(24);
    }

    [Fact]
    public void test_array2d_call()
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
        var arr = Parser.ParseExpression<Array2dExpr>(mzn);
        arr.I.ShouldBe(12);
        arr.J.ShouldBe(12);
        arr.Elements.Count.ShouldBe(144);
    }

    [Fact]
    public void test_expr_type_inst()
    {
        var mzn = "record(1..1:x): a";
        var node = Parser.ParseItem<DeclareItem>(mzn);
        var record = node.Type as RecordTypeSyntax;
        var param = record!.Fields[0];
        param.Name.ToString().ShouldBe("x");
        param.Type.ToString().ShouldBe("1..1");
    }

    [Fact]
    public void test_parse_let_xd()
    {
        var mzn = """
            let {
                array[1..1] of var bool: res;
                constraint res[1];
            } in res;
            """;
        var let = Parser.ParseExpression<LetExpr>(mzn);
        let.Locals.Count.ShouldBe(2);
        let.Body.ShouldBeOfType<IdentExpr>();
        let.Body.ToString().ShouldBe("res");
    }

    [Fact]
    public void test_partial_range_ti()
    {
        var mzn = "0..: xd;";
        var node = Parser.ParseItem<DeclareItem>(mzn);
        var type = node.Type as ExprTypeSyntax;
        var expr = type!.Expr;
        expr.ShouldBeOfType<RangeExpr>();
        var rng = (RangeExpr)expr;
        rng.Upper.ShouldBeNull();
        // rng.Lower.ShouldBe(new IntLiteralSyntax(0));
    }

    [Fact]
    public void test_record_comp()
    {
        var mzn = """
            [
              i: (a: some_map[i], b: some_map[i] mod 2 = 0) | i in Some
            ]
            """;
        var expr = Parser.ParseExpression<ArrayCompExpr>(mzn);
    }

    [Fact]
    public void test_set_of_ti()
    {
        var mzn = "set of var int: xd";
        var node = Parser.ParseItem<DeclareItem>(mzn);
        node.Name.ToString().ShouldBe("xd");
    }

    [Fact]
    public void test_postfix_range_operator()
    {
        var mzn = "var 0..: xd";
        var node = Parser.ParseItem<DeclareItem>(mzn);
        node.Name.ToString().ShouldBe("xd");
        var type = (ExprTypeSyntax)node.Type;
        type.IsVar.ShouldBeTrue();
        var range = (RangeExpr)type.Expr;
        range.Lower.ShouldBeOfType<IntExpr>();
        range.Upper.ShouldBeNull();
        var lo = (IntExpr)range.Lower!;
        lo.Value.ShouldBe(0);
    }

    [Fact]
    public void test_array3d_literal()
    {
        var mzn = "[| |1,1|1,1|, |2,2|2,2|, |3,3|3,3| |]";
        var arr = Parser.ParseExpression<Array3dSyntax>(mzn);
        arr.I.ShouldBe(3);
        arr.J.ShouldBe(2);
        arr.K.ShouldBe(2);
        arr.Elements.Count.ShouldBe(12);
        var numbers = arr.Elements.Select(x => (int)(IntExpr)x);
        numbers.ShouldBe([1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3]);
    }

    [Fact]
    public void test_array3d_empty()
    {
        var mzn = "[| || |]";
        var arr = Parser.ParseExpression<Array3dSyntax>(mzn);
        arr.I.ShouldBe(0);
        arr.J.ShouldBe(0);
        arr.K.ShouldBe(0);
        arr.Elements.ShouldBeNull();
    }

    [Theory]
    [InlineData("annotation xd")]
    [InlineData("annotation something(int: x)")]
    public void test_annotation_declaration(string mzn)
    {
        var ann = Parser.ParseItem<DeclareItem>(mzn);
        ann.Type.Kind.ShouldBe(TypeKind.TYPE_ANNOTATION);
    }

    [Theory]
    // [InlineData("1+2", 3)]
    // [InlineData("1+2*4", 9)]
    // [InlineData("(1+2)*4", 12)]
    [InlineData("2*4-1", 7)]
    public void test_operator_precedence(string mzn, int expected)
    {
        var expr = Parser.ParseExpression<MiniZincExpr>(mzn);
        int eval(MiniZincSyntax expr)
        {
            if (expr is IntExpr i)
                return i.Value;

            if (expr is not BinOpExpr binop)
                throw new Exception();

            var left = eval(binop.Left);
            var right = eval(binop.Right);
            switch (binop.Operator)
            {
                case TOKEN_PLUS:
                    return left + right;
                case TOKEN_MINUS:
                    return left - right;
                case TOKEN_TIMES:
                    return left * right;
                case TOKEN_DIVIDE:
                    return left / right;
                default:
                    throw new Exception();
            }
        }
        var result = eval(expr);
        result.ShouldBe(expected);
    }

    [Fact]
    public void test_float_range()
    {
        var mzn = "0.01..1.123;";
        var expr = Parser.ParseExpression<RangeExpr>(mzn);
        expr.Lower.ShouldBeOfType<FloatExpr>();
        expr.Upper.ShouldBeOfType<FloatExpr>();
    }

    [Fact]
    public void test_parse_operator_same_precedence()
    {
        var mzn = "a diff (b union c)";
        var expr = Parser.ParseExpression<BinOpExpr>(mzn);
        var ozn = expr.Write();
        ozn.ShouldBe("a diff (b union c)");
    }

    [Fact]
    public void test_union_type()
    {
        var mzn = @"tuple(int) ++ tuple(int): i";
        var expr = Parser.ParseItem<DeclareItem>(mzn);
        expr.Name.ToString().ShouldBe("i");
        var type = (CompositeTypeSyntax)expr.Type;
        type.Types.Count.ShouldBe(2);
    }

    [Fact]
    public void test_parse_unary_prec()
    {
        var mzn = @"not(A -> B) -> not(C -> D)";
        var expr = Parser.ParseExpression<BinOpExpr>(mzn);
        expr.Left.ShouldBeOfType<UnOpExpr>();
        expr.Right.ShouldBeOfType<UnOpExpr>();
    }

    [Fact]
    public void test_parse_precedence_left_assoc_equal()
    {
        var mzn = "1 + 2 - 3";
        var binop = Parser.ParseExpression<BinOpExpr>(mzn);
        binop.Operator.ShouldBe(TOKEN_MINUS);
        var oz = binop.Write();
        oz.ShouldBe(mzn);
        var b = 2;
    }

    [Fact]
    public void test_parse_precedence_left_assoc_descending()
    {
        var mzn = "a - b >= c";
        var binop = Parser.ParseExpression<BinOpExpr>(mzn);
        binop.Operator.ShouldBe(TOKEN_GREATER_THAN_EQUAL);
        var oz = binop.Write();
        oz.ShouldBe(mzn);
    }

    [Fact]
    public void test_write_precedence_left_assoc_brackets()
    {
        var mzn = "(Formula[1] > 0) == assignment[1]";
        var binop = Parser.ParseExpression<BinOpExpr>(mzn);
        binop.Operator.ShouldBe(TOKEN_EQUAL);
        var oz = binop.Write();
        oz.ShouldBe(mzn);
    }

    [Fact]
    public void test_parse_right_assoc()
    {
        var mzn = "var MyTuple ++ var MyTuple: tuptup = tup ++ tup;";
        var binop = Parser.ParseItem<DeclareItem>(mzn);
        var oz = binop.Write();
        oz.ShouldBeEquivalentTo(mzn);
    }

    [Fact]
    public void test_parse_union_type_arg()
    {
        var mzn = "var ..-1 union {1,3}";
        var parser = new Parser(mzn);
        var ok = parser.ParseType(out var type);
        ok.ShouldBeTrue();
        var a = 2;
    }

    [Theory]
    [InlineData("-2.8421709430404e-14")]
    [InlineData("2e10")]
    public void test_parse_float(string f)
    {
        var mzn = f;
        var node = Parser.ParseExpression<FloatExpr>(mzn);
        var a = 2;
    }

    [Fact]
    public void test_parse_generic_func()
    {
        var mzn = "$T: foo(tuple($T): x) = x.1;";
        var parser = new Parser(mzn);
        var ok = parser.ParseDeclareOrAssignStatement(out var node);
        ok.ShouldBeTrue();
    }
}
