namespace MiniZinc.Model.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open MiniZinc.Parsers

module ``Parser Tests`` =
    
    // Test parsing the string, it is sanitized first
    let testParser (parser: P<'t>) (input: string) =

        let source, comments =
            Parse.stripComments input
        
        let parsed =
            match Parse.stringWith parser source with
            | Ok x -> x
            | Error err -> failwith (string err)
        
        ()
            
    // Test parsing the string, it is sanitized first
    let testRoundtrip (parser: P<'t>) (input: string) (writer: MiniZincEncoder -> 't -> MiniZincEncoder) =

        let source, comments =
            Parse.stripComments input
        
        let parsed =
            match Parse.stringWith parser source with
            | Ok x -> x
            | Error err -> failwith (string err)
            
        let encoder = MiniZincEncoder()
        let write = (writer encoder)
        write parsed
        
        let encoded =
            encoder.String
       
        let roundtrip =
            match Parse.stringWith parser encoded with
            | Ok x -> x
            | Error err -> failwith (string err)
        
        ()
        
    
    [<Theory>]
    [<InlineData("a")>]
    [<InlineData("B")>]
    [<InlineData("_A_NAME")>]
    [<InlineData("aN4m3w1thnumb3r5")>]
    [<InlineData("'A name with Quotes'")>]
    let ``test identifier`` arg =
        testRoundtrip Parsers.id arg (fun enc -> enc.write)
        
    [<Theory>]
    [<InlineData("int")>]
    [<InlineData("var int")>]
    [<InlineData("var set of int")>]
    [<InlineData("opt bool")>]
    [<InlineData("set of opt float")>]
    [<InlineData("var X")>]
    [<InlineData("par set of 'something weird'")>]
    let ``test base type inst`` arg =
        testRoundtrip Parsers.base_ti_expr arg (fun enc -> enc.writeTypeInst)
        
    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` arg =
        testRoundtrip Parsers.array_ti_expr arg (fun enc -> enc.writeTypeInst)
        
    [<Theory>]
    [<InlineData("record(a: int, bool:b)")>]
    [<InlineData("record(c: X, set of int: d)")>]
    let ``test record type inst`` arg =
        testParser Parsers.record_ti arg
                
    [<Theory>]
    [<InlineData("tuple(int, string, string)")>]
    [<InlineData("tuple(X, 'something else', set of Q)")>]
    let ``test tuple type inst`` arg =
        testParser Parsers.tuple_ti arg
        
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
        testRoundtrip Parsers.ti_expr_and_id arg (fun enc -> enc.write)
            
    [<Theory>]
    [<InlineData("enum A = {A1}")>]
    [<InlineData("enum B = {_A, _B, _C}")>]
    [<InlineData("enum C = {  'One', 'Two',   'Three'}")>]
    let ``test enum`` arg =
        testRoundtrip Parsers.enum_item arg (fun enc -> enc.writeEnum)
    
    [<Theory>]
    [<InlineData("type A = record(a: int)")>]
    [<InlineData("type B = int")>]
    [<InlineData("type C = tuple(bool, tuple(int, string))")>]
    let ``test type alias`` arg =
        testRoundtrip Parsers.alias_item arg (fun enc -> enc.writeSynonym)
        
    [<Theory>]
    [<InlineData("1")>]
    [<InlineData("2.0")>]
    [<InlineData("aVariable")>]
    [<InlineData("(1)")>]
    [<InlineData("(  (3))")>]
    let ``test num expr atom simple`` arg =
        testRoundtrip Parsers.num_expr_atom_head arg (fun enc -> enc.writeNumExpr)
        
    [<Theory>]
    [<InlineData("-100")>]
    [<InlineData("+300.2")>]
    let ``test num unary op`` arg =
        testRoundtrip Parsers.num_expr_atom_head arg (fun enc -> enc.writeNumExpr)
        
    [<Theory>]
    [<InlineData("100 + 100")>]
    [<InlineData("100 / 1.0")>]
    [<InlineData("A `something` B")>]
    let ``test num binary op`` arg =
        testRoundtrip Parsers.num_expr_atom_head arg (fun enc -> enc.writeNumExpr)
        
    [<Theory>]
    [<InlineData("% 12312312")>]
    [<InlineData("/* wsomethign */")>]
    let ``test comments`` arg =
        let output = Parse.stringWith Parsers.comment arg
        output.AssertOk()
        
    [<Theory>]
    [<InlineData("let {int: a = 2} in a;")>]
    let ``test let`` arg =
        testRoundtrip Parsers.expr arg (fun enc -> enc.writeExpr)
        
    [<Theory>]
    [<InlineData("array2d(ROW, COL, []);")>]
    let ``test call`` arg =
        testRoundtrip Parsers.call_expr arg (fun enc -> enc.writeCall)
        
    [<Theory>]
    [<InlineData("[];")>]
    [<InlineData("[1,2,3,];")>]
    [<InlineData("[true, false, X, true];")>]
    let ``test array1d literal`` arg =
        testRoundtrip Parsers.array1d_literal arg (fun enc -> enc.write)
        
    
    let ``test generator`` arg =
        testRoundtrip Parsers.gen_call_expr arg
        
    [<Theory>]
    [<InlineData("var llower..lupper: Production;")>]
    let test_xd arg =
        testRoundtrip Parsers.var_decl_item arg (fun enc -> enc.write)
        
    [<Fact>]
    let ``test array2d literal`` () =
        let input = """[|
        0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
     | 0, 0, _, _, _, _, 0, _, _, _, 0
     | 0, _, 0, _, _, _, _, _, _, _, 0
     | 0, _, _, _, _, _, _, _, _, _, 0
     | 0, 0, _, _, _, _, _, _, 0, _, 0
     | 0, _, _, _, _, _, _, _, _, _, 0
     | 0, _, 0, _, 0, _, _, _, _, _, 0
     | 0, _, _, 0, _, _, _, _, _, _, 0
     | 0, _, _, _, _, _, _, _, _, _, 0
     | 0, _, _, _, _, _, _, _, _, _, 0
     | 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
     |]"""
        testRoundtrip Parsers.array2d_literal input (fun enc -> enc.write)
        
                
    [<Theory>]
    [<InlineData("a = array1d(0..z, [x*x | x in 0..z]);")>]
    [<InlineData("a = 1..10;")>]
    let ``test range expr `` arg =
        testParser Parsers.ast arg 
        
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
        testRoundtrip Parsers.set_comp input (fun enc -> enc.write)

    [<Theory>]
    [<InlineData("{r | r in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i] } }")>]
    let ``test set comp`` arg =
        testRoundtrip Parsers.set_comp arg
        
    [<Fact>]
    let ``test forall`` () =
        let input = """
    forall(i in 1..nb, j in i+1..nb)
    (
        card(sets[i] intersect sets[j]) <= 1
    );
"""
        testRoundtrip Parsers.gen_call_expr input (fun enc -> enc.writeGenCall)
        
    [<Fact>]
    let ``test output``() =
        let input = """
output
  [ if i = n_stores then "\n]"
    elseif i mod 5 = 0 then ",\n"
    else ","
    endif
  ];
  """
        testParser Parsers.ast input