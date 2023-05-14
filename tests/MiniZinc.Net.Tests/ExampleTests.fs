module MiniZinc.Net.Tests.ExampleTests

open MiniZinc
open Xunit
open System.IO

let example_dir =
    $"{__SOURCE_DIRECTORY__}/examples"

type TestSpec =
    { Name   : string 
    ; File   : FileInfo
    ; String : string }
    
    static member create name =
        let file = FileInfo $"{example_dir}/{name}.mzn"
        let string = File.ReadAllText file.FullName
        let spec = { Name = name ; File = file; String = string }
        spec
    
let test (spec: TestSpec) =
    match Model.parseString spec.String with
    | Ok model ->
        model
    | Error err->
        let trace = err.Trace
        failwith err.Message

[<Fact>]
let ``test 2DPacking`` () =
    let spec = TestSpec.create "2DPacking" 
    let result = test spec
    ()
    

[<Fact>]
let ``test alpha`` () =
    let spec = TestSpec.create "alpha" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships10`` () =
    let spec = TestSpec.create "battleships10" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_1`` () =
    let spec = TestSpec.create "battleships_1" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_2`` () =
    let spec = TestSpec.create "battleships_2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_3`` () =
    let spec = TestSpec.create "battleships_3" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_4`` () =
    let spec = TestSpec.create "battleships_4" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_5`` () =
    let spec = TestSpec.create "battleships_5" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_7`` () =
    let spec = TestSpec.create "battleships_7" 
    let result = test spec
    ()
    

[<Fact>]
let ``test battleships_9`` () =
    let spec = TestSpec.create "battleships_9" 
    let result = test spec
    ()
    

[<Fact>]
let ``test blocksworld_instance_1`` () =
    let spec = TestSpec.create "blocksworld_instance_1" 
    let result = test spec
    ()
    

[<Fact>]
let ``test blocksworld_instance_2`` () =
    let spec = TestSpec.create "blocksworld_instance_2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test cutstock`` () =
    let spec = TestSpec.create "cutstock" 
    let result = test spec
    ()
    

[<Fact>]
let ``test eq20`` () =
    let spec = TestSpec.create "eq20" 
    let result = test spec
    ()
    

[<Fact>]
let ``test factory_planning_instance`` () =
    let spec = TestSpec.create "factory_planning_instance" 
    let result = test spec
    ()
    

[<Fact>]
let ``test golomb`` () =
    let spec = TestSpec.create "golomb" 
    let result = test spec
    ()
    

[<Fact>]
let ``test halfreif`` () =
    let spec = TestSpec.create "halfreif" 
    let result = test spec
    ()
    

[<Fact>]
let ``test jobshop2x2`` () =
    let spec = TestSpec.create "jobshop2x2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test knights`` () =
    let spec = TestSpec.create "knights" 
    let result = test spec
    ()
    

[<Fact>]
let ``test langford`` () =
    let spec = TestSpec.create "langford" 
    let result = test spec
    ()
    

[<Fact>]
let ``test langford2`` () =
    let spec = TestSpec.create "langford2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test latin_squares_fd`` () =
    let spec = TestSpec.create "latin_squares_fd" 
    let result = test spec
    ()
    

[<Fact>]
let ``test magicsq_3`` () =
    let spec = TestSpec.create "magicsq_3" 
    let result = test spec
    ()
    

[<Fact>]
let ``test magicsq_4`` () =
    let spec = TestSpec.create "magicsq_4" 
    let result = test spec
    ()
    

[<Fact>]
let ``test magicsq_5`` () =
    let spec = TestSpec.create "magicsq_5" 
    let result = test spec
    ()
    

[<Fact>]
let ``test multidimknapsack_simple`` () =
    let spec = TestSpec.create "multidimknapsack_simple" 
    let result = test spec
    ()
    

[<Fact>]
let ``test oss`` () =
    let spec = TestSpec.create "oss" 
    let result = test spec
    ()
    

[<Fact>]
let ``test packing`` () =
    let spec = TestSpec.create "packing" 
    let result = test spec
    ()
    

[<Fact>]
let ``test perfsq`` () =
    let spec = TestSpec.create "perfsq" 
    let result = test spec
    ()
    

[<Fact>]
let ``test perfsq2`` () =
    let spec = TestSpec.create "perfsq2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test photo`` () =
    let spec = TestSpec.create "photo" 
    let result = test spec
    ()
    

[<Fact>]
let ``test product_fd`` () =
    let spec = TestSpec.create "product_fd" 
    let result = test spec
    ()
    

[<Fact>]
let ``test product_lp`` () =
    let spec = TestSpec.create "product_lp" 
    let result = test spec
    ()
    

[<Fact>]
let ``test quasigroup_qg5`` () =
    let spec = TestSpec.create "quasigroup_qg5" 
    let result = test spec
    ()
    

[<Fact>]
let ``test queen_cp2`` () =
    let spec = TestSpec.create "queen_cp2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test queen_ip`` () =
    let spec = TestSpec.create "queen_ip" 
    let result = test spec
    ()
    

[<Fact>]
let ``test radiation`` () =
    let spec = TestSpec.create "radiation" 
    let result = test spec
    ()
    

[<Fact>]
let ``test simple_sat`` () =
    let spec = TestSpec.create "simple_sat" 
    let result = test spec
    ()
    

[<Fact>]
let ``test singHoist2`` () =
    let spec = TestSpec.create "singHoist2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test steiner-triples`` () =
    let spec = TestSpec.create "steiner-triples" 
    let result = test spec
    ()
    

[<Fact>]
let ``test sudoku`` () =
    let spec = TestSpec.create "sudoku" 
    let result = test spec
    ()
    

[<Fact>]
let ``test template_design`` () =
    let spec = TestSpec.create "template_design" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_1`` () =
    let spec = TestSpec.create "tenpenki_1" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_2`` () =
    let spec = TestSpec.create "tenpenki_2" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_3`` () =
    let spec = TestSpec.create "tenpenki_3" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_4`` () =
    let spec = TestSpec.create "tenpenki_4" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_5`` () =
    let spec = TestSpec.create "tenpenki_5" 
    let result = test spec
    ()
    

[<Fact>]
let ``test tenpenki_6`` () =
    let spec = TestSpec.create "tenpenki_6" 
    let result = test spec
    ()
    

[<Fact>]
let ``test timetabling`` () =
    let spec = TestSpec.create "timetabling" 
    let result = test spec
    ()
    

[<Fact>]
let ``test trucking`` () =
    let spec = TestSpec.create "trucking" 
    let result = test spec
    ()
    

[<Fact>]
let ``test warehouses`` () =
    let spec = TestSpec.create "warehouses" 
    let result = test spec
    ()
    

[<Fact>]
let ``test wolf_goat_cabbage`` () =
    let spec = TestSpec.create "wolf_goat_cabbage" 
    let result = test spec
    ()
    

[<Fact>]
let ``test zebra`` () =
    let spec = TestSpec.create "zebra" 
    let result = test spec
    ()
    