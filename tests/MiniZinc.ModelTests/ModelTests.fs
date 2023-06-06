namespace MiniZinc.Model.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO
open System

module ModelTests =
    
    [<Fact>]
    let ``test binding conflict`` () =
        
        let x = Expr.Int 1
        let y = Expr.Int 2
        
        let binding =
            Bindings.empty
            |> Bindings.add "x" (Binding.Assign x)
            |> Bindings.add "x" (Binding.Assign y)
            |> Map.find "x"
            
        match binding with
        | Conflict xs ->
            ()
        | _ ->
            failwith "xd"

    [<Fact>]
    let ``test binding override`` () =
        let x = Expr.Int 1
        
        let binding =
            Bindings.empty
            |> Bindings.add "x" (Binding.Assign x)
            |> Bindings.add "x" (Binding.Assign x)
            |> Map.find "x"
            
        match binding with
        | Binding.Assign x ->
            ()
        | _ ->
            failwith "xd"
            
    [<Fact>]
    let ``test assign variable`` () =
        
        let ti =
            { Type = BaseType.Int
            ; Inst = Inst.Var
            ; IsSet = false
            ; IsOpt = false }
            
        let expr =
            Expr.Int 100
        
        let bindings =
            Bindings.empty
            |> Bindings.add "x" (Binding.Declare {Name="x"; Annotations = []; Type=ti; Expr=None})
            |> Bindings.add "x" (Binding.Assign expr)
            
        let binding =
            bindings
            |> Map.find "x"
            
        match binding with
        | Binding.Declare {Type=ti_; Expr = Some expr_} ->
            ti_ ?= ti
            expr_ ?= expr
        | _ ->
            failwith "xd"

    [<Theory>]
    [<InlineData("var int: x;")>]
    let ``test detect unassigned`` arg =
        
        let model =
            Model.ParseString arg
            
        assert model.Value.Unassigned.ContainsKey "x"
        
    [<Theory>]
    [<InlineData("var int: x;x=100;")>]
    let ``test detect unassigned 2`` arg =

        let model =
            Model.ParseString arg
        
        assert model.Value.Unassigned.IsEmpty
                                        