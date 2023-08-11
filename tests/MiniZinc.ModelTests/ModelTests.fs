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

module ``Model Tests`` =
    
    let parseOptions =
        { ParseOptions.Default with Debug = true }   
    
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
            { TypeInst.Empty with
                Name = "x"
                Type = Type.Int
                IsVar = true }
            
        let expr =
            Expr.Int 100
        
        let bindings =
            NameSpace.empty
            |> NameSpace.addDeclare ti
            |> NameSpace.add "x" (Binding.Expr expr)
            |> NameSpace.bindings
            
        let binding = bindings["x"]
        match binding with
        | Binding.Variable v ->
            v.Value.Value ?= expr
        | _ ->
            failwith "xd"

    [<Theory>]
    [<InlineData("int: x;")>]
    let ``test detect unassigned`` arg =
        
        let model =
            Model.ParseString arg
            
        assert model.Get().NameSpace.Undeclared.IsEmpty
        
    [<Theory>]
    [<InlineData("x=100")>]
    let ``test detect unassigned 2`` arg =

        let model =
            parseWith Parsers.item parseOptions arg
            
        model.AssertOk()                                        