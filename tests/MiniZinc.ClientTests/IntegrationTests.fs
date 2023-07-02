namespace MiniZinc.Tests

open MiniZinc
open Xunit

[<CollectionDefinition("Integration Tests", DisableParallelization = true)>]
type ``Integration Tests``(fixture: ClientFixture) =
        
    let client = fixture.Client
            
    let test (name: string) =
        let suite = TestSuite.load name
        let model = TestSuite.parseModel suite
        let iface = client.GetModelInterface(model)
        let types = client.GetModelTypes(model)
        let solution = client.SolveSync(model)
        
        let cases =
            suite.Tests
            |> List.collect (fun test -> test.Expected)
            |> List.filter (fun test -> test.StatusType = solution.StatusType)
            |> function
                | [] ->
                    failwith $"No cases matched result {solution.StatusType}"
                | xs ->
                    xs
                
        let success =            
            cases
            |> List.filter (fun test -> test.Objective = solution.Objective)
            |> function
                | [] ->
                    failwith $"No cases matched objective {solution.Objective}"
                | xs ->
                    xs
        
        solution.IsSuccess.AssertEquals(true)
        ()
        
    interface IClassFixture<ClientFixture>

    [<Fact>]
    member this. ``test 2DPacking`` () =
        test "2DPacking.mzn"

    [<Fact>]
    member this. ``test alpha`` () =
        test "alpha.mzn"

    [<Fact>]
    member this. ``test battleships10`` () =
        test "battleships10.mzn"


    [<Fact>]
    member this. ``test battleships 1`` () =
        test "battleships_1.mzn"


    [<Fact>]
    member this. ``test battleships 2`` () =
        test "battleships_2.mzn"


    [<Fact>]
    member this. ``test battleships 3`` () =
        test "battleships_3.mzn"


    [<Fact>]
    member this. ``test battleships 4`` () =
        test "battleships_4.mzn"


    [<Fact>]
    member this. ``test battleships 5`` () =
        test "battleships_5.mzn"


    [<Fact>]
    member this. ``test battleships 7`` () =
        test "battleships_7.mzn"


    [<Fact>]
    member this. ``test battleships 9`` () =
        test "battleships_9.mzn"


    [<Fact>]
    member this. ``test blocksworld instance 1`` () =
        test "blocksworld_instance_1.mzn"


    [<Fact>]
    member this. ``test blocksworld instance 2`` () =
        test "blocksworld_instance_2.mzn"


    [<Fact>]
    member this. ``test cutstock`` () =
        test "cutstock.mzn"


    [<Fact>]
    member this. ``test eq20`` () =
        test "eq20.mzn"


    [<Fact>]
    member this. ``test factory planning instance`` () =
        test "factory_planning_instance.mzn"


    [<Fact>]
    member this. ``test golomb`` () =
        test "golomb.mzn"


    [<Fact>]
    member this. ``test halfreif`` () =
        test "halfreif.mzn"


    [<Fact>]
    member this. ``test jobshop2x2`` () =
        test "jobshop2x2.mzn"


    [<Fact>]
    member this. ``test knights`` () =
        test "knights.mzn"


    [<Fact>]
    member this. ``test langford`` () =
        test "langford.mzn"


    [<Fact>]
    member this. ``test langford2`` () =
        test "langford2.mzn"


    [<Fact>]
    member this. ``test latin squares fd`` () =
        test "latin_squares_fd.mzn"


    [<Fact>]
    member this. ``test magicsq 3`` () =
        test "magicsq_3.mzn"


    [<Fact>]
    member this. ``test magicsq 4`` () =
        test "magicsq_4.mzn"


    [<Fact>]
    member this. ``test magicsq 5`` () =
        test "magicsq_5.mzn"


    [<Fact>]
    member this. ``test multidimknapsack simple`` () =
        test "multidimknapsack_simple.mzn"


    [<Fact>]
    member this. ``test oss`` () =
        test "oss.mzn"


    [<Fact>]
    member this. ``test packing`` () =
        test "packing.mzn"


    [<Fact>]
    member this. ``test perfsq`` () =
        test "perfsq.mzn"


    [<Fact>]
    member this. ``test perfsq2`` () =
        test "perfsq2.mzn"


    [<Fact>]
    member this. ``test photo`` () =
        test "photo.mzn"


    [<Fact>]
    member this. ``test product fd`` () =
        test "product_fd.mzn"


    [<Fact>]
    member this. ``test product lp`` () =
        test "product_lp.mzn"


    [<Fact>]
    member this. ``test quasigroup qg5`` () =
        test "quasigroup_qg5.mzn"


    [<Fact>]
    member this. ``test queen cp2`` () =
        test "queen_cp2.mzn"


    [<Fact>]
    member this. ``test queen ip`` () =
        test "queen_ip.mzn"


    [<Fact>]
    member this. ``test radiation`` () =
        test "radiation.mzn"


    [<Fact>]
    member this. ``test simple sat`` () =
        test "simple_sat.mzn"


    [<Fact>]
    member this. ``test singHoist2`` () =
        test "singHoist2.mzn"


    [<Fact>]
    member this. ``test steiner-triples`` () =
        test "steiner-triples.mzn"


    [<Fact>]
    member this. ``test sudoku`` () =
        test "sudoku.mzn"


    [<Fact>]
    member this. ``test template design`` () =
        test "template_design.mzn"


    [<Fact>]
    member this. ``test tenpenki 1`` () =
        test "tenpenki_1.mzn"


    //[<Fact(Skip="Slow")>]
    [<Fact>]
    member this. ``test tenpenki 2`` () =
        test "tenpenki_2.mzn"

    
    [<Fact>]
    member this. ``test tenpenki 3`` () =
        test "tenpenki_3.mzn"


    [<Fact>]
    member this. ``test tenpenki 4`` () =
        test "tenpenki_4.mzn"


    [<Fact>]
    member this. ``test tenpenki 5`` () =
        test "tenpenki_5.mzn"


    [<Fact>]
    member this. ``test tenpenki 6`` () =
        test "tenpenki_6.mzn"


    [<Fact>]
    member this. ``test timetabling`` () =
        test "timetabling.mzn"


    [<Fact>]
    member this. ``test trucking`` () =
        test "trucking.mzn"


    [<Fact>]
    member this. ``test warehouses`` () =
        test "warehouses.mzn"


    [<Fact>]
    member this. ``test wolf goat cabbage`` () =
        test "wolf_goat_cabbage.mzn"


    [<Fact>]
    member this. ``test zebra`` () =
        test "zebra.mzn"
        
