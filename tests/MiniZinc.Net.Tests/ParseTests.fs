﻿namespace MiniZinc.Net.Tests

open MiniZinc
open Xunit

module ParseTests =
         
    let parse p s =
        match Parse.parseString p s with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg            
    
    [<Theory>]
    [<InlineData("a")>]
    [<InlineData("B")>]
    [<InlineData("_A_NAME")>]
    [<InlineData("aN4m3w1thnumb3r5")>]
    [<InlineData("'A name with Quotes'")>]
    let ``test identifier`` arg =
        let input = arg
        let output = parse Parse.ident input
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
        let output = parse Parse.base_ti_expr input
        ()
        
    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` arg =
        let input = arg
        let output = parse Parse.array_ti_expr input
        ()
        
    [<Theory>]
    [<InlineData("record(a: int, bool:b)")>]
    [<InlineData("record(c: X, set of int: d)")>]
    let ``test record type inst`` arg =
        let input = arg
        let output = parse Parse.record_ti input
        ()
                
    [<Theory>]
    [<InlineData("tuple(int, string, string)")>]
    [<InlineData("tuple(X, 'something else', set of Q)")>]
    let ``test tuple type inst`` arg =
        let input = arg
        let output = parse Parse.tuple_ti input
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
        let output = parse Parse.ti_expr_and_id input
        ()
        
            
    [<Theory>]
    [<InlineData("enum A = {A1}")>]
    [<InlineData("enum B = {_A, _B, _C}")>]
    [<InlineData("enum C = {  'One', 'Two',   'Three'}")>]
    let ``test enum`` arg =
        let input = arg
        let output = parse Parse.enum_item input
        ()        
        
    
    [<Theory>]
    [<InlineData("type A = record(a: int)")>]
    [<InlineData("type B = int")>]
    [<InlineData("type C = tuple(bool, tuple(int, string))")>]
    let ``test type alias`` arg =
        let input = arg
        let output = parse Parse.alias_item input
        ()
        
    [<Theory>]
    [<InlineData("1")>]
    [<InlineData("2.0")>]
    [<InlineData("aVariable")>]
    [<InlineData("(1)")>]
    [<InlineData("(  (3))")>]
    let ``test num expr atom simple`` arg =
        let input = arg
        let output = parse Parse.num_expr_atom_head input
        ()
        
    [<Theory>]
    [<InlineData("-100")>]
    [<InlineData("+300.2")>]
    let ``test num unary op`` arg =
        let input = arg
        let output = parse Parse.num_expr_atom_head input
        ()        
        
    [<Theory>]
    [<InlineData("100 + 100")>]
    let ``test num binary op`` arg =
        let input = arg
        let output = parse Parse.num_expr_atom_head input
        ()        
        
    // [<Theory>]
    // [<InlineData("let {int: a = 2} in a;")>]
    // let ``test num expr`` arg =
    //     let input = arg
    //     let output = parse Parse.num_expr_atom_head input
    //     ()
        