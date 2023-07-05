(*

ModelTests.fs

Tests regarding the creation and manipulation of
Model objects.
*)

namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit

module ``Parser Tests`` =
    
    // Test parsing the string, it is sanitized first
    let testParser (parser: Parser<'t>) (input: string) =

        let source, comments =
            parseComments input
        
        let parsed =
            match parseString parser source with
            | Result.Ok x -> x
            | Result.Error err -> failwith (string err)
        
        ()
            
    // Test parsing the string, it is sanitized first
    let testRoundtrip (parser: Parser<'t>) (input: string) (writer: Encoder -> 't -> unit) =

        let source, comments =
            parseComments input
        
        let parsed =
            match parseString parser source with
            | Result.Ok x ->
                x
            | Result.Error err ->
                failwith (string err)
            
        let encoder = Encoder()
        let write = (writer encoder)
        write parsed
        
        let encoded =
            encoder.String
       
        let roundtrip =
            match parseString parser encoded with
            | Result.Ok x ->
                x
            | Result.Error err ->
                failwith (string err)
        
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
        testRoundtrip Parsers.ti_expr_and_id arg (fun enc -> enc.writeParameter)
            
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
        let output = parseString Parsers.comment arg
        output.AssertOk()
        
    [<Theory>]
    [<InlineData("let {int: a = 2} in a;")>]
    let ``test let`` arg =
        testRoundtrip Parsers.let_expr arg (fun enc -> enc.writeLetExpr)
        
    [<Theory>]
    [<InlineData("array2d(ROW, COL, []);")>]
    let ``test call`` arg =
        testRoundtrip Parsers.call_expr arg (fun enc -> enc.writeCall)
        
    [<Theory>]
    [<InlineData("[];")>]
    [<InlineData("[1,2,3,];")>]
    [<InlineData("[true, false, X, true];")>]
    let ``test array1d literal`` arg =
        testRoundtrip Parsers.array1d_literal arg (fun enc -> enc.writeArray1d)
        
    [<Theory>]
    [<InlineData("""constraint
	forall (i in 1..n-1) (
		if d[i] == d[i+1] then
			lex_lesseq([p[i,  j] | j in 1..t],
				[p[i+1,j] | j in 1..t])
		else
			true
		endif
	);""")>]
    [<InlineData("""constraint
	forall(i in 1..n-1) (
		if d[i] < d[i+1] then
		       sum (j in 1..t) (p[i,j]*R[j])
		     < sum (j in 1..t) (p[i+1,j]*R[j])
		else
			true
		endif
	);""")>]
    let ``test gen call`` arg =
        testRoundtrip Parsers.constraint_item arg (fun enc -> enc.writeConstraintItem)
        
    [<Theory>]
    [<InlineData("var llower..lupper: Production;")>]
    let test_xd arg =
        testRoundtrip Parsers.var_decl_item arg (fun enc -> enc.writeVariable)
        
    [<Theory>]
    [<InlineData("[|0 , 0, 0, | _, 1, _ | 3, 2, _|]")>]
    [<InlineData("[| |]")>]
    [<InlineData("[| true, false, true |]")>]
    let ``test array2d literal`` arg =
        testRoundtrip Parsers.array2d_literal arg (fun enc -> enc.writeArray2d)
                
    [<Theory>]
    [<InlineData("a = array1d(0..z, [x*x | x in 0..z]);")>]
    [<InlineData("a = 1..10;")>]
    let ``test range expr `` arg =
        testParser Parsers.model arg 
        
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
        testRoundtrip Parsers.set_comp input (fun enc -> enc.writeSetComp)

    [<Theory>]
    [<InlineData("{r | r in {R0 + [-1, -2, -2, -1,  1,  2,  2,  1][i] } }")>]
    let ``test set comp`` arg =
        testRoundtrip Parsers.set_comp arg

    [<Theory>]
    [<InlineData("forall( i in 1..nb, j in i+1..nb ) (card(sets[i] intersect sets[j]) <= 1);")>]
    [<InlineData("sum( k in 1..K ) ( bin[k] );")>]
    [<InlineData("forall(k in 1 .. K)(is_feasible_packing(bin[k], [item[k, j] | j in 1 .. N]));")>]
    [<InlineData("forall (i in 1..n-1) (if d[i] == d[i+1] then lex_lesseq([p[i,  j] | j in 1..t], [p[i+1,j] | j in 1..t]) else true endif);")>]
    let ``test gencall`` input =
        testRoundtrip
            Parsers.gen_call_expr
            input
            (fun enc -> enc.writeGenCall)
        
    [<Theory>]
    [<InlineData("""
    output 
    [ "Cost = ",  show( obj ), "\n" ] ++ 
    [ "Pieces = \n\t" ] ++ [show(pieces)] ++ [ "\n" ] ++ 
    [ "Items = \n\t" ] ++
    [ show(items[k, i]) ++ if k = K then "\n\t" else " " endif |
        i in 1..N, k in 1..K ] ++
    [ "\n" ];""")>]                
    let ``test output``input =
        testRoundtrip
            Parsers.output_item
            input
            (fun enc -> enc.writeOutputItem)
            
    [<Theory>]
    [<InlineData("annotation f(string:x)")>]
    let ``test annotation item`` input =
        testRoundtrip
            (Parsers.annotation_item)
            input
            (fun enc -> enc.writeAnnotationItem)
        
