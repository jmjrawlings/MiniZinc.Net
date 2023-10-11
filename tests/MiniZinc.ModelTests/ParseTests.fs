(*

ModelTests.fs

Tests regarding the creation and manipulation of
Model objects.
*)

namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Parser
open MiniZinc.Tests
open Xunit
open FParsec

module ``Parser Tests`` =
    
    let parseOptions =
        ParseOptions.Default
                
    // Test parsing the string
    let testRoundtrip (parser: Parser<'t>) (mzn: string) (writer: Encoder -> 't -> unit) =
        
        let parser =
            spaces >>. parser .>> spaces .>> eof
            
        let parsed =
            match parseWith parser parseOptions mzn with
            | Result.Ok x ->
                x
            | Result.Error err ->
                let trace = err.Trace
                failwith $"""
Failed to parse the test model:
---------------------------------------------------------

{mzn}

---------------------------------------------------------

{err.Message}

{err.Trace}

"""
            
        let encoder = Encoder()
        let write = (writer encoder)
        write parsed
        
        let encoded =
            encoder.String.Trim()
       
        match parseWith parser parseOptions encoded with
        | Result.Ok x ->
            x
        | Result.Error err ->
            failwith $"""
Failed to parse the roundtripped MZN:
---------------------------------------------------------
Original:

{mzn}

---------------------------------------------------------
Encoded:

{encoded}

---------------------------------------------------------

{err.Message}

{err.Trace}
"""
    let testExpr mzn =
        testRoundtrip Parsers.expr mzn (fun enc -> enc.writeExpr)
        
    let testItem mzn =
        testRoundtrip Parsers.item mzn (fun enc -> enc.writeItem)
    
    [<Theory>]
    [<InlineData("a")>]
    [<InlineData("B")>]
    [<InlineData("_A_NAME")>]
    [<InlineData("aN4m3w1thnumb3r5")>]
    [<InlineData("'A name with Quotes'")>]
    let ``test valid identifier`` mzn =
        testRoundtrip Parsers.ident mzn (fun enc -> enc.write)
    
    [<Theory>]
    [<InlineData(""" "abc" """)>]
    [<InlineData(""" "Escaped single \'quotes\' are fine" """)>]
    [<InlineData(""" "Escaped double \"quotes\" are fine" """)>]
    let ``test string literal`` (mzn:string) =
        let mzn = mzn.Trim()
        testRoundtrip Parsers.string_lit mzn (fun enc -> enc.writeString)
    
    [<Theory>]
    [<InlineData("[")>]
    [<InlineData("@")>]
    let ``test invalid identifier`` mzn =
        let result = Parser.parseWith Parsers.ident parseOptions mzn
        result.AssertErr()
        
    [<Theory>]
    [<InlineData("int")>]
    [<InlineData("any $T")>]
    [<InlineData("var int")>]
    [<InlineData("opt bool")>]
    [<InlineData("set of opt float")>]
    [<InlineData("var X")>]
    let ``test base type inst`` mzn =
        testRoundtrip Parsers.base_ti_expr mzn (fun enc -> enc.writeTypeInst)
    
    [<InlineData("par set of 'something weird'")>]
    [<InlineData("var set of string")>]
    let ``test set type inst`` mzn =
        testRoundtrip Parsers.base_ti_expr mzn (fun enc -> enc.writeTypeInst)
       
    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` mzn =
        testRoundtrip Parsers.array_ti_expr mzn (fun enc -> enc.writeTypeInst)
        
    [<Theory>]
    [<InlineData("record(a: int, bool:b)")>]
    [<InlineData("record(c: X, set of int: d)")>]
    let ``test record type inst`` mzn =
        testRoundtrip
            Parsers.base_ti_expr_tail
            mzn
            (fun enc -> enc.writeType)
                
    [<Theory>]
    [<InlineData("tuple(int, string, string)")>]
    [<InlineData("tuple(X, 'something else', set of Q)")>]
    let ``test tuple type inst`` mzn =
        testRoundtrip Parsers.base_ti_expr_tail mzn (fun enc -> enc.writeType)
        
    [<Theory>]
    [<InlineData("var MyTuple ++ var MyTuple")>]
    [<InlineData("array[a+1..b+3] of var int")>]
    let ``test complex type inst`` mzn =
        testRoundtrip Parsers.ti_expr mzn (fun enc -> enc.writeTypeInst)
            
    [<Theory>]
    [<InlineData("var set of bool: xd")>]
    [<InlineData("var set of opt 'Quoted': 'something else'")>]
    [<InlineData("XYZ123: xyz123")>]
    [<InlineData("par opt 'A Thing': _ABC")>]
    [<InlineData("int: x")>]
    [<InlineData("var int: x")>]
    [<InlineData("var set of int: x")>]
    [<InlineData("opt bool: x")>]
    [<InlineData("set of opt float: x")>]
    [<InlineData("var X: x")>]
    [<InlineData("par set of 'something weird': okay")>]
    [<InlineData("array[int] of int: A_B_C")>]
    [<InlineData("array[int,int] of var float: _A2")>]
    [<InlineData("array[5..9] of set of bool: x2")>]
    [<InlineData("record(a: int, bool:b): 'asdf '")>]
    [<InlineData("record(c: X, set of int: d): asdf")>]
    [<InlineData("tuple(int, string, string): x")>]
    [<InlineData("tuple(X, 'something else', set of Q): Q")>]
    let ``test type inst and id`` mzn =
        testRoundtrip Parsers.ti_expr_and_id mzn (fun enc -> enc.writeParameter)
            
    [<Theory>]
    [<InlineData("enum A = {A1}")>]
    [<InlineData("enum B = {_A, _B, _C}")>]
    [<InlineData("enum C = {  'One', 'Two',   'Three'}")>]
    [<InlineData("enum D")>]
    let ``test enum`` mzn =
        testRoundtrip Parsers.item mzn (fun enc -> enc.writeItem)
    
    [<Theory>]
    [<InlineData("type A = record(a: int)")>]
    [<InlineData("type B = int")>]
    [<InlineData("type C = tuple(bool, tuple(int, string))")>]
    let ``test type alias`` mzn =
        testRoundtrip Parsers.item mzn (fun enc -> enc.writeItem)
        
    [<Theory>]
    [<InlineData("1")>]
    [<InlineData("2.0")>]
    [<InlineData("aVariable")>]
    [<InlineData("(1)")>]
    [<InlineData("(  (3))")>]
    let ``test num expr atom simple`` mzn =
        testRoundtrip Parsers.expr_atom_head mzn (fun enc -> enc.writeExpr)
        
    [<Theory>]
    [<InlineData("-100")>]
    [<InlineData("+300.2")>]
    let ``test num unary op`` mzn =
        testRoundtrip Parsers.expr mzn (fun enc -> enc.writeExpr)
        
    [<Theory>]
    [<InlineData("100 + 100")>]
    [<InlineData("100 / 1.0")>]
    [<InlineData("A `something` B")>]
    let ``test num binary op`` mzn =
        testRoundtrip Parsers.expr mzn (fun enc -> enc.writeExpr)
        
    [<Theory>]
    [<InlineData("predicate x(var $T: x)")>]
    let ``test generic param`` mzn =
        testItem mzn
        
    [<Theory>]
    [<InlineData("% 12312312")>]
    let ``test comments`` mzn =
        let output = parseWith Parsers.line_comment parseOptions mzn
        output.AssertOk()
        
    [<Theory>]
    [<InlineData("/* something */")>]
    let ``test block comment`` mzn =
        let output = parseWith Parsers.block_comment parseOptions mzn
        output.AssertOk()
        
    [<Theory>]
    [<InlineData("let {int: a = 2} in a")>]
    [<InlineData("let {int: x = ceil(z);} in x")>]
    [<InlineData("let {constraint x < 100;} in x")>]
    [<InlineData("let {int: x = ceil(z);constraint x < 100;} in x")>]
    let ``test let`` mzn =
        testExpr mzn
        
    [<Theory>]
    [<InlineData("array2d(ROW, COL, [])")>]
    let ``test call`` mzn =
        testExpr mzn
        
    [<Theory>]
    [<InlineData("[]")>]
    [<InlineData("[1,2,3,]")>]
    [<InlineData("[true, false, X, true]")>]
    [<InlineData("[1, _, 3, _, 5]")>]
    [<InlineData("[<>, _, 10, q]")>]
    let ``test array1d literal`` mzn =
        testExpr mzn
    
    [<Theory>]
    [<InlineData("[| |]")>]
    [<InlineData("[| one_item |]")>]
    [<InlineData("[| 0 , 0, 0, | _, 1, _ | 3, 2, _ |]")>]
    [<InlineData("[| 1,2,3,4 | 5,6,7,8 | 9,10,11,12 |]")>]
    [<InlineData("[| true, false, true\n | |]")>]
    let ``test array2d literal`` mzn =
        testExpr mzn
        
    [<Theory>]
    [<InlineData("[ 1:2, 3:4]")>]
    [<InlineData("[ 1:a, 3:b, (3+1): 4]")>]
    [<InlineData("[ X:1, Y:2, 3, 4, 5]")>]
    let ``test array1d indexed literal`` mzn =
        testExpr mzn
    
    [<Theory>]
    [<InlineData("[| | | |]", 1, 0, 0)>]
    [<InlineData("[| | one_item | |]", 1, 1, 1)>]
    [<InlineData("[| | 1, 2 |, | 3, 4 | |]", 2, 1, 2)>]
    [<InlineData("[| | 1, 2 | 3, 4|, | 5, 6|7, 8| |]", 2, 2, 2)>]
    let ``test array3d literal`` (mzn, i, j, k) =
        let array =
            match testExpr mzn with
            | Expr.Array3DLit arr -> arr | _ -> Array3D.zeroCreate 0 0 0
        i.AssertEquals(Array3D.length1 array)
        j.AssertEquals(Array3D.length2 array)
        k.AssertEquals(Array3D.length3 array)
        
    [<Theory>]
    [<InlineData("if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif")>]
    let ``test if then else`` mzn =
        testExpr mzn
                
    [<Theory>]
    [<InlineData("1..10")>]
    [<InlineData("-1 .. z")>]
    [<InlineData("a+1..b+3")>]
    [<InlineData("x[1]..x[2]")>]
    [<InlineData("f(x)..15")>]
    let ``test range ti `` mzn =
        testRoundtrip Parsers.base_ti_expr_tail mzn (fun enc -> enc.writeType)
        
    [<Fact>]
    let test_bad () =
        let input = """
        { n * (R - 1) + C
        |
            i in 1..8,
            R in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i]},
            C in {C0 + [-2, -1,  1,  2,  2,  1, -1, -2][i]}
            where R in row /\ C in col
        }"""
        testRoundtrip Parsers.expr input (fun enc -> enc.writeExpr)

    [<Theory>]
    [<InlineData("{}")>]
    [<InlineData("{1}")>]
    [<InlineData("{1, <>}")>]
    [<InlineData("{true, false, _, true}")>]
    let ``test set lit`` mzn =
        testExpr mzn
    
    [<Theory>]
    [<InlineData("{r | r in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i] } }")>]
    let ``test set comp`` mzn =
        testExpr mzn

    [<Theory>]
    [<InlineData("forall( i in 1..nb, j in i+1..nb ) (card(sets[i] intersect sets[j]) <= 1)")>]
    [<InlineData("sum( k in 1..K ) ( bin[k] )")>]
    [<InlineData("forall(k in 1 .. K)(is_feasible_packing(bin[k], [item[k, j] | j in 1 .. N]))")>]
    [<InlineData("forall (i in 1..n-1) (if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif)")>]
    let ``test generator call`` mzn =
        testExpr mzn
        
    [<Theory>]
    [<InlineData("""
    output 
        [ "Cost = ",  show( obj ), "\n" ] ++ 
        [ "Pieces = \n\t" ] ++ [show(pieces)] ++ [ "\n" ] ++ 
        [ "Items = \n\t" ] ++
        [ show(items[k, i]) ++ if k = K then "\n\t" else " " endif |
            i in 1..N, k in 1..K ] ++
        [ "\n" ]
    """)>]
    [<InlineData("""
        output ["% a = ", show(a), ";\n", "b = ", show(b), ";\n"]
    """)>]
    let ``test output`` mzn =
        testRoundtrip Parsers.item mzn (fun enc -> enc.writeItem)
            
    [<Theory>]
    [<InlineData("annotation f(string:x)")>]
    let ``test annotation item`` input =
        testRoundtrip Parsers.item input (fun enc -> enc.writeItem)
    
    [<Fact>]
    let ``test annotations`` ()=
        let input = ":: add_to_output :: mzn_break_here"
        testRoundtrip Parsers.annotations input (fun enc -> enc.writeAnnotations)
        
    [<Theory>]
    [<InlineData("tuple(1..3): x = (4,)")>]
    let ``test declare tuple`` mzn =
        testItem mzn
            
    [<Theory>]
    [<InlineData("x.1")>]
    [<InlineData("x.b")>]
    let ``test item access`` mzn =
        testExpr mzn
            
    [<Theory>]
    [<InlineData("int:a::add_to_output = product([|2, 2 | 2, 2|])")>]
    [<InlineData("""output ["Escaped single \'quotes\' are fine."]""")>]
    let ``test items`` mzn =
        testItem mzn
    
    [<Theory>]
    [<InlineData("/* this is a block comment */")>]
    [<InlineData("% this is a line comment */")>]
    [<InlineData("/* this has % things /  in it */")>]
    let ``test comment`` mzn =
        let statement, comments = Parser.parseComments mzn
        statement.AssertEmpty("")
        
    [<Theory>]
    [<InlineData("""constraint x > 2 -> not z""")>]
    let ``test constraint`` mzn =
        testItem mzn
        
            