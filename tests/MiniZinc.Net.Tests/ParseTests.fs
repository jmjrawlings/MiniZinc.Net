namespace MiniZinc.Net.Tests

open MiniZinc
open Xunit

module ParseTests =
         
    
    [<Theory>]
    [<InlineData("a")>]
    [<InlineData("B")>]
    [<InlineData("_A_NAME")>]
    [<InlineData("aN4m3w1thnumb3r5")>]
    [<InlineData("'A name with Quotes'")>]
    let ``test identifier`` arg =
        let input = arg
        let output = test_parser Parsers.ident input
        ()    
        
    [<Theory>]
    [<InlineData("int")>]
    [<InlineData("var int")>]
    [<InlineData("var set of int")>]
    [<InlineData("opt bool")>]
    [<InlineData("set of opt float")>]
    [<InlineData("var X")>]
    [<InlineData("par set of 'something weird'")>]
    let ``test base type inst`` arg =
        let input = arg
        let output = test_parser Parsers.base_ti_expr input
        ()
        
    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` arg =
        let input = arg
        let output = test_parser Parsers.array_ti_expr input
        ()
        
    [<Theory>]
    [<InlineData("record(a: int, bool:b)")>]
    [<InlineData("record(c: X, set of int: d)")>]
    let ``test record type inst`` arg =
        let input = arg
        let output = test_parser Parsers.record_ti input
        ()
                
    [<Theory>]
    [<InlineData("tuple(int, string, string)")>]
    [<InlineData("tuple(X, 'something else', set of Q)")>]
    let ``test tuple type inst`` arg =
        let input = arg
        let output = test_parser Parsers.tuple_ti input
        ()
        
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
    [<InlineData("record(a: int, bool:b): 'asdf '")>]
    [<InlineData("record(c: X, set of int: d): asdf")>]
    [<InlineData("tuple(int, string, string): x")>]
    [<InlineData("tuple(X, 'something else', set of Q): Q")>]
    let ``test type inst and id`` arg =
        let input = arg
        let output = test_parser Parsers.ti_expr_and_id input
        ()
        
            
    [<Theory>]
    [<InlineData("enum A = {A1}")>]
    [<InlineData("enum B = {_A, _B, _C}")>]
    [<InlineData("enum C = {  'One', 'Two',   'Three'}")>]
    let ``test enum`` arg =
        let input = arg
        let output = test_parser Parsers.enum_item input
        ()        
        
    
    [<Theory>]
    [<InlineData("type A = record(a: int)")>]
    [<InlineData("type B = int")>]
    [<InlineData("type C = tuple(bool, tuple(int, string))")>]
    let ``test type alias`` arg =
        let input = arg
        let output = test_parser Parsers.alias_item input
        ()
        
    [<Theory>]
    [<InlineData("1")>]
    [<InlineData("2.0")>]
    [<InlineData("aVariable")>]
    [<InlineData("(1)")>]
    [<InlineData("(  (3))")>]
    let ``test num expr atom simple`` arg =
        let input = arg
        let output = test_parser Parsers.num_expr_atom_head input
        ()
        
    [<Theory>]
    [<InlineData("-100")>]
    [<InlineData("+300.2")>]
    let ``test num unary op`` arg =
        let input = arg
        let output = test_parser Parsers.num_expr_atom_head input
        ()        
        
    [<Theory>]
    [<InlineData("100 + 100")>]
    [<InlineData("100 / 1.0")>]
    [<InlineData("A `something` B")>]
    let ``test num binary op`` arg =
        let input = arg
        let output = test_parser Parsers.num_expr_atom_head input
        ()
        
    [<Theory>]
    [<InlineData("% 12312312")>]
    [<InlineData("/* wsomethign */")>]
    let ``test comments`` arg =
        let input = arg
        let output = test_parser Parsers.comment input
        ()
        
        
    [<Theory>]
    [<InlineData("let {int: a = 2} in a;")>]
    let ``test let`` arg =
        let input = arg
        let output = test_parser Parsers.expr input
        ()
        
    [<Theory>]
    [<InlineData("array2d(ROW, COL, []);")>]
    let ``test call`` arg =
        let input = arg
        let output = test_parser Parsers.call_expr input
        ()
        
    [<Theory>]
    [<InlineData("[];")>]
    [<InlineData("[||];")>]
    [<InlineData("[|1,2,3|2,3,4|4,5,6|];")>]
    [<InlineData("[true, false, X, true];")>]
    let ``test array literals`` arg =
        let input = arg
        let output = test_parser Parsers.expr input
        ()
        
    [<Theory>]
    [<InlineData("constraint (
( ( Formula[1] > 0 ) == assignment[1] ) \/ 
  ( ( Formula[2] > 0 ) == assignment[2] )
 );")>]
    let ``test xd`` arg =
        let input = arg
        let output = test_parser Parsers.model input
        ()        
        