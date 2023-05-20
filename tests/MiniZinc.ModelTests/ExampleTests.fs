namespace MiniZinc.Model.Tests

open MiniZinc
open Xunit
open System.IO

module ExampleTests =
  
    let test (name: string) =
        let file = FileInfo $"examples/{name}.mzn"
        let model = Model.parseFile file
        model.AssertOk()


    [<Fact>]
    let ``test 2DPacking`` () =
        test "2DPacking"


    [<Fact>]
    let ``test alpha`` () =
        test "alpha"


    [<Fact>]
    let ``test battleships10`` () =
        test "battleships10"


    [<Fact>]
    let ``test battleships_1`` () =
        test "battleships_1"


    [<Fact>]
    let ``test battleships_2`` () =
        test "battleships_2"


    [<Fact>]
    let ``test battleships_3`` () =
        test "battleships_3"


    [<Fact>]
    let ``test battleships_4`` () =
        test "battleships_4"


    [<Fact>]
    let ``test battleships_5`` () =
        test "battleships_5"


    [<Fact>]
    let ``test battleships_7`` () =
        test "battleships_7"


    [<Fact>]
    let ``test battleships_9`` () =
        test "battleships_9"


    [<Fact>]
    let ``test blocksworld_instance_1`` () =
        test "blocksworld_instance_1"


    [<Fact>]
    let ``test blocksworld_instance_2`` () =
        test "blocksworld_instance_2"


    [<Fact>]
    let ``test cutstock`` () =
        test "cutstock"


    [<Fact>]
    let ``test eq20`` () =
        test "eq20"


    [<Fact>]
    let ``test factory_planning_instance`` () =
        test "factory_planning_instance"


    [<Fact>]
    let ``test golomb`` () =
        test "golomb"


    [<Fact>]
    let ``test halfreif`` () =
        test "halfreif"


    [<Fact>]
    let ``test jobshop2x2`` () =
        test "jobshop2x2"


    [<Fact>]
    let ``test knights`` () =
        test "knights"


    [<Fact>]
    let ``test langford`` () =
        test "langford"


    [<Fact>]
    let ``test langford2`` () =
        test "langford2"


    [<Fact>]
    let ``test latin_squares_fd`` () =
        test "latin_squares_fd"


    [<Fact>]
    let ``test magicsq_3`` () =
        test "magicsq_3"


    [<Fact>]
    let ``test magicsq_4`` () =
        test "magicsq_4"


    [<Fact>]
    let ``test magicsq_5`` () =
        test "magicsq_5"


    [<Fact>]
    let ``test multidimknapsack_simple`` () =
        test "multidimknapsack_simple"


    [<Fact>]
    let ``test oss`` () =
        test "oss"


    [<Fact>]
    let ``test packing`` () =
        test "packing"


    [<Fact>]
    let ``test perfsq`` () =
        test "perfsq"


    [<Fact>]
    let ``test perfsq2`` () =
        test "perfsq2"


    [<Fact>]
    let ``test photo`` () =
        test "photo"


    [<Fact>]
    let ``test product_fd`` () =
        test "product_fd"


    [<Fact>]
    let ``test product_lp`` () =
        test "product_lp"


    [<Fact>]
    let ``test quasigroup_qg5`` () =
        test "quasigroup_qg5"


    [<Fact>]
    let ``test queen_cp2`` () =
        test "queen_cp2"


    [<Fact>]
    let ``test queen_ip`` () =
        test "queen_ip"


    [<Fact>]
    let ``test radiation`` () =
        test "radiation"


    [<Fact>]
    let ``test simple_sat`` () =
        test "simple_sat"


    [<Fact>]
    let ``test singHoist2`` () =
        test "singHoist2"


    [<Fact>]
    let ``test steiner-triples`` () =
        test "steiner-triples"


    [<Fact>]
    let ``test sudoku`` () =
        test "sudoku"


    [<Fact>]
    let ``test template_design`` () =
        test "template_design"


    [<Fact>]
    let ``test tenpenki_1`` () =
        test "tenpenki_1"


    [<Fact>]
    let ``test tenpenki_2`` () =
        test "tenpenki_2"


    [<Fact>]
    let ``test tenpenki_3`` () =
        test "tenpenki_3"


    [<Fact>]
    let ``test tenpenki_4`` () =
        test "tenpenki_4"


    [<Fact>]
    let ``test tenpenki_5`` () =
        test "tenpenki_5"


    [<Fact>]
    let ``test tenpenki_6`` () =
        test "tenpenki_6"


    [<Fact>]
    let ``test timetabling`` () =
        test "timetabling"


    [<Fact>]
    let ``test trucking`` () =
        test "trucking"


    [<Fact>]
    let ``test warehouses`` () =
        test "warehouses"


    [<Fact>]
    let ``test wolf_goat_cabbage`` () =
        test "wolf_goat_cabbage"


    [<Fact>]
    let ``test zebra`` () =
        test "zebra"
