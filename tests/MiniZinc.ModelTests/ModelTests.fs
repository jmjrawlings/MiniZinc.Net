(*

ModelTests.fs

Tests regarding the creation and manipulation of
Model objects.

*)

namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit

module ``Model Tests`` =
    
    [<Fact>]
    let ``test binding conflict`` () =
        
        let x = Expr.Int 1
        let y = Expr.Int 2
        
        let binding =
            NameSpace.empty
            |> NameSpace.add "x" (Binding.Expr x)
            |> NameSpace.add "x" (Binding.Expr y)
            |> NameSpace.bindings
            |> Map.find "x"
            
        match binding with
        | Binding.Multiple xs ->
            ()
        | _ ->
            failwith "xd"

    [<Fact>]
    let ``test binding override`` () =
        let x = Expr.Int 1
        
        let binding =
            NameSpace.empty
            |> NameSpace.add "x" (Binding.Expr x)
            |> NameSpace.add "x" (Binding.Expr x)
            |> NameSpace.bindings
            |> Map.find "x"
            
        match binding with
        | Binding.Expr x ->
            ()
        | _ ->
            failwith "xd"
            
    [<Fact>]
    let ``test assign variable`` () =
        
        let ti =
            { Type = Type.Int
            ; Inst = Inst.Var
            ; IsSet = false
            ; IsArray = false 
            ; IsOptional = false }
            
        let expr =
            Expr.Int 100
        
        let bindings =
            NameSpace.empty
            |> NameSpace.add "x" (Binding.Variable {Name="x"; Annotations = []; TypeInst=ti; Expr=None})
            |> NameSpace.add "x" (Binding.Expr expr)
            |> NameSpace.bindings
            
        let binding =
            bindings
            |> Map.find "x"
            
        match binding with
        | Binding.Variable {TypeInst=ti_; Expr = Some expr_} ->
            ti_ ?= ti
            expr_ ?= expr
        | _ ->
            failwith "xd"

    [<Theory>]
    [<InlineData("var int: x;")>]
    let ``test detect unassigned`` arg =
        
        let model =
            Model.ParseString arg
            
        assert model.Model.NameSpace.Undeclared.IsEmpty
        
    [<Theory>]
    [<InlineData("var int: x;x=100;")>]
    let ``test detect unassigned 2`` arg =

        let model =
            Model.ParseString arg
        
        assert model.Model.NameSpace.Undeclared.IsEmpty
                                        