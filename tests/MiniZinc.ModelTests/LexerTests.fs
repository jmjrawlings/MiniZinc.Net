(*

ModelTests.fs

Tests regarding the creation and manipulation of
Model objects.
*)

namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open FParsec

module ``Lexer Tests`` =
                    
    // Test lexing the given string
    let test (mzn: string) =
        
        let result = Lexer.lexString mzn
        match result.Error with
        | "" ->
            ()
        | err ->
                failwith $"""
Failed to lex the test string:
---------------------------------------------------------

{mzn}

---------------------------------------------------------

{err}

"""
     
    [<Theory>]
    [<InlineData("a")>]
    [<InlineData("B")>]
    [<InlineData("_A_NAME")>]
    [<InlineData("aN4m3w1thnumb3r5")>]
    [<InlineData("'A name with Quotes'")>]
    let ``test identifer`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData(""" "abc" """)>]
    [<InlineData(""" "Escaped single \'quotes\' are fine" """)>]
    [<InlineData(""" "Escaped double \"quotes\" are fine" """)>]
    let ``test string literal`` mzn =
        test mzn

    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("record(a: int, bool:b)")>]
    [<InlineData("record(c: X, set of int: d)")>]
    let ``test record type inst`` mzn =
        test mzn
                
    [<Theory>]
    [<InlineData("tuple(int, string, string)")>]
    [<InlineData("tuple(X, 'something else', set of Q)")>]
    let ``test tuple type inst`` mzn =
        test mzn
            
        
    [<Theory>]
    [<InlineData("var MyTuple ++ var MyTuple")>]
    [<InlineData("array[a+1..b+3] of var int")>]
    let ``test complex type inst`` mzn =
        test mzn
        
            
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
        test mzn
            
    [<Theory>]
    [<InlineData("enum A = {A1}")>]
    [<InlineData("enum B = {_A, _B, _C}")>]
    [<InlineData("enum C = {  'One', 'Two',   'Three'}")>]
    [<InlineData("enum D")>]
    let ``test enum`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData("type A = record(a: int)")>]
    [<InlineData("type B = int")>]
    [<InlineData("type C = tuple(bool, tuple(int, string))")>]
    let ``test type alias`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("1")>]
    [<InlineData("2.0")>]
    [<InlineData("aVariable")>]
    [<InlineData("(1)")>]
    [<InlineData("(  (3))")>]
    let ``test num expr atom simple`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("-100")>]
    [<InlineData("+300.2")>]
    let ``test num unary op`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("100 + 100")>]
    [<InlineData("100 / 1.0")>]
    [<InlineData("A `something` B")>]
    let ``test num binary op`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("predicate x(var $T: x)")>]
    let ``test generic param`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("% 12312312")>]
    let ``test line comments`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("/* something */")>]
    let ``test block comment`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("let {int: a = 2} in a")>]
    [<InlineData("let {int: x = ceil(z);} in x")>]
    [<InlineData("let {constraint x < 100;} in x")>]
    [<InlineData("let {int: x = ceil(z);constraint x < 100;} in x")>]
    let ``test let`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("array2d(ROW, COL, [])")>]
    let ``test call`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("[]")>]
    [<InlineData("[1,2,3,]")>]
    [<InlineData("[true, false, X, true]")>]
    [<InlineData("[1, _, 3, _, 5]")>]
    [<InlineData("[<>, _, 10, q]")>]
    let ``test array1d literal`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData("[| |]")>]
    [<InlineData("[| one_item |]")>]
    [<InlineData("[| 0 , 0, 0, | _, 1, _ | 3, 2, _ |]")>]
    [<InlineData("[| 1,2,3,4 | 5,6,7,8 | 9,10,11,12 |]")>]
    [<InlineData("[| true, false, true\n | |]")>]
    let ``test array2d literal`` mzn =
        test mzn
        
    [<Theory>]
    [<InlineData("[ 1:2, 3:4]")>]
    [<InlineData("[ 1:a, 3:b, (3+1): 4]")>]
    [<InlineData("[ X:1, Y:2, 3, 4, 5]")>]
    let ``test array1d indexed literal`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData("[| | | |]", 1, 0, 0)>]
    [<InlineData("[| | one_item | |]", 1, 1, 1)>]
    [<InlineData("[| | 1, 2 |, | 3, 4 | |]", 2, 1, 2)>]
    [<InlineData("[| | 1, 2 | 3, 4|, | 5, 6|7, 8| |]", 2, 2, 2)>]
    let ``test array3d literal`` (mzn, i, j, k) =
        test mzn
        
    [<Theory>]
    [<InlineData("if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif")>]
    let ``test if then else`` mzn =
        test mzn
                
    [<Theory>]
    [<InlineData("1..10")>]
    [<InlineData("-1 .. z")>]
    [<InlineData("a+1..b+3")>]
    [<InlineData("x[1]..x[2]")>]
    [<InlineData("f(x)..15")>]
    let ``test range ti `` mzn =
        test mzn

    [<Theory>]
    [<InlineData("{}")>]
    [<InlineData("{1}")>]
    [<InlineData("{1, <>}")>]
    [<InlineData("{true, false, _, true}")>]
    let ``test set lit`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData("{r | r in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i] } }")>]
    let ``test set comp`` mzn =
        test mzn

    [<Theory>]
    [<InlineData("forall( i in 1..nb, j in i+1..nb ) (card(sets[i] intersect sets[j]) <= 1)")>]
    [<InlineData("sum( k in 1..K ) ( bin[k] )")>]
    [<InlineData("forall(k in 1 .. K)(is_feasible_packing(bin[k], [item[k, j] | j in 1 .. N]))")>]
    [<InlineData("forall (i in 1..n-1) (if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif)")>]
    let ``test generator call`` mzn =
        test mzn
        
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
        test mzn
    
    [<Theory>]
    [<InlineData("tuple(1..3): x = (4,)")>]
    let ``test declare tuple`` mzn =
        test mzn
            
    [<Theory>]
    [<InlineData("x.1")>]
    [<InlineData("x.b")>]
    let ``test item access`` mzn =
        test mzn
            
    [<Theory>]
    [<InlineData("int:a::add_to_output = product([|2, 2 | 2, 2|])")>]
    [<InlineData("""output ["Escaped single \'quotes\' are fine."]""")>]
    let ``test items`` mzn =
        test mzn
    
    [<Theory>]
    [<InlineData("""constraint x > 2 -> not z""")>]
    let ``test constraint`` mzn =
        test mzn
        
            