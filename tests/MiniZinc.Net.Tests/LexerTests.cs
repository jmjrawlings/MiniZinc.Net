namespace MiniZinc.Tests;

public class LexerTests
{
    // Test lexing the given string
    void Test(string mzn)
    {
        var lexer = Lexer.FromString(mzn);
        var tokens = lexer.ReadToEnd().ToArray();
        var a = 1;
    }

    [Theory]
    [InlineData("a")]
    [InlineData("B")]
    [InlineData("_A_NAME")]
    [InlineData("aN4m3w1thnumb3r5")]
    [InlineData("'A name with Quotes'")]
    public void Test_identifer(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData(""" "abc" """)]
    [InlineData(""" "Escaped single \'quotes\' are fine" """)]
    [InlineData(""" "Escaped double \"quotes\" are fine" """)]
    public void test_string_literal(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("array[int] of int")]
    [InlineData("array[int,int] of var float")]
    public void test_array_type_inst(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("record(a: int, bool:b)")]
    [InlineData("record(c: X, set of int: d)")]
    public void test_record_type_inst(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("tuple(int, string, string)")]
    [InlineData("tuple(X, 'something else', set of Q)")]
    public void test_tuple_type_inst(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("var MyTuple ++ var MyTuple")]
    [InlineData("array[a+1..b+3] of var int")]
    public void test_complex_type_inst(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("var set of bool: xd")]
    [InlineData("var set of opt 'Quoted': 'something else'")]
    [InlineData("XYZ123: xyz123")]
    [InlineData("par opt 'A Thing': _ABC")]
    [InlineData("int: x")]
    [InlineData("var int: x")]
    [InlineData("var set of int: x")]
    [InlineData("opt bool: x")]
    [InlineData("set of opt float: x")]
    [InlineData("var X: x")]
    [InlineData("par set of 'something weird': okay")]
    [InlineData("array[int] of int: A_B_C")]
    [InlineData("array[int,int] of var float: _A2")]
    [InlineData("array[5..9] of set of bool: x2")]
    [InlineData("record(a: int, bool:b): 'asdf '")]
    [InlineData("record(c: X, set of int: d): asdf")]
    [InlineData("tuple(int, string, string): x")]
    [InlineData("tuple(X, 'something else', set of Q): Q")]
    public void test_type_inst_and_id(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("enum A = {A1}")]
    [InlineData("enum B = {_A, _B, _C}")]
    [InlineData("enum C = {  'One', 'Two',   'Three'}")]
    [InlineData("enum D")]
    public void test_enum(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("type A = record(a: int)")]
    [InlineData("type B = int")]
    [InlineData("type C = tuple(bool, tuple(int, string))")]
    public void test_type_alias(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2.0")]
    [InlineData("aVariable")]
    [InlineData("(1)")]
    [InlineData("(  (3))")]
    public void test_num_expr_atom_simple(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("-100")]
    [InlineData("+300.2")]
    public void test_num_unary_op(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("100 + 100")]
    [InlineData("100 / 1.0")]
    [InlineData("A `something` B")]
    public void test_num_binary_op(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("predicate x(var $T: x)")]
    public void test_generic_param(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("% 12312312")]
    public void test_line_comments(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("/* something */")]
    public void test_block_comment(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("let {int: a = 2} in a")]
    [InlineData("let {int: x = ceil(z);} in x")]
    [InlineData("let {constraint x < 100;} in x")]
    [InlineData("let {int: x = ceil(z);constraint x < 100;} in x")]
    public void test_let(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("array2d(ROW, COL, [])")]
    public void test_call(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("[1,2,3,]")]
    [InlineData("[true, false, X, true]")]
    [InlineData("[1, _, 3, _, 5]")]
    [InlineData("[<>, _, 10, q]")]
    public void test_array1d_literal(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("[| |]")]
    [InlineData("[| one_item |]")]
    [InlineData("[| 0 , 0, 0, | _, 1, _ | 3, 2, _ |]")]
    [InlineData("[| 1,2,3,4 | 5,6,7,8 | 9,10,11,12 |]")]
    [InlineData("[| true, false, true\n | |]")]
    public void test_array2d_literal(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("[ 1:2, 3:4]")]
    [InlineData("[ 1:a, 3:b, (3+1): 4]")]
    [InlineData("[ X:1, Y:2, 3, 4, 5]")]
    public void test_array1d_indexed_literal(string mzn)
    {
        Test(mzn);
    }

    // [Theory]
    // [InlineData("[| | | |]", 1, 0, 0)]
    // [InlineData("[| | one_item | |]", 1, 1, 1)]
    // [InlineData("[| | 1, 2 |, | 3, 4 | |]", 2, 1, 2)]
    // [InlineData("[| | 1, 2 | 3, 4|, | 5, 6|7, 8| |]", 2, 2, 2)]
    // public void test_array3d_literal (mzn, i, j, k) =
    //     Test(mzn);

    [Theory]
    [InlineData(
        "if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif"
    )]
    public void test_if_then_else(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("1..10")]
    [InlineData("-1 .. z")]
    [InlineData("a+1..b+3")]
    [InlineData("x[1]..x[2]")]
    [InlineData("f(x)..15")]
    public void test_range_ti(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{1}")]
    [InlineData("{1, <>}")]
    [InlineData("{true, false, _, true}")]
    public void test_set_lit(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("{r | r in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i] } }")]
    public void test_set_comp(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("forall( i in 1..nb, j in i+1..nb ) (card(sets[i] intersect sets[j]) <= 1)")]
    [InlineData("sum( k in 1..K ) ( bin[k] )")]
    [InlineData("forall(k in 1 .. K)(is_feasible_packing(bin[k], [item[k, j] | j in 1 .. N]))")]
    [InlineData(
        "forall (i in 1..n-1) (if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif)"
    )]
    public void test_generator_call(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData(
        """
                output
                    [ "Cost = ",  show( obj ), "\n" ] ++
                    [ "Pieces = \n\t" ] ++ [show(pieces)] ++ [ "\n" ] ++
                    [ "Items = \n\t" ] ++
                    [ show(items[k, i]) ++ if k = K then "\n\t" else " " endif |
                        i in 1..N, k in 1..K ] ++
                    [ "\n" ]
                """
    )]
    [InlineData(
        """
                    output ["% a = ", show(a), ";\n", "b = ", show(b), ";\n"]
                """
    )]
    public void test_output(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("tuple(1..3): x = (4,)")]
    public void test_declare_tuple(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("x.1")]
    [InlineData("x.b")]
    public void test_item_access(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("int:a::add_to_output = product([|2, 2 | 2, 2|])")]
    [InlineData("""output ["Escaped single \'quotes\' are fine."]""")]
    public void test_items(string mzn)
    {
        Test(mzn);
    }

    [Theory]
    [InlineData("""constraint x > 2 -> not z""")]
    public void test_constraint(string mzn)
    {
        Test(mzn);
    }
}
