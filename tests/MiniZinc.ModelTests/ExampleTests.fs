
namespace MiniZinc.Model.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module ExampleTests =
   
    let test (name: string) =
        let suite = TestSuite.load name
        let model = TestSuite.parseModel suite
        model.Value.Undeclared.AssertEmpty()
        model.Value.Conflicts.AssertEmpty()


    [<Fact>]
    let ``test 2DPacking`` () =
        test "2DPacking.mzn"


    [<Fact>]
    let ``test alpha`` () =
        test "alpha.mzn"


    [<Fact>]
    let ``test battleships10`` () =
        test "battleships10.mzn"


    [<Fact>]
    let ``test battleships_1`` () =
        test "battleships_1.mzn"


    [<Fact>]
    let ``test battleships_2`` () =
        test "battleships_2.mzn"


    [<Fact>]
    let ``test battleships_3`` () =
        test "battleships_3.mzn"


    [<Fact>]
    let ``test battleships_4`` () =
        test "battleships_4.mzn"


    [<Fact>]
    let ``test battleships_5`` () =
        test "battleships_5.mzn"


    [<Fact>]
    let ``test battleships_7`` () =
        test "battleships_7.mzn"


    [<Fact>]
    let ``test battleships_9`` () =
        test "battleships_9.mzn"


    [<Fact>]
    let ``test blocksworld_instance_1`` () =
        test "blocksworld_instance_1.mzn"


    [<Fact>]
    let ``test blocksworld_instance_2`` () =
        test "blocksworld_instance_2.mzn"


    [<Fact>]
    let ``test cutstock`` () =
        test "cutstock.mzn"


    [<Fact>]
    let ``test eq20`` () =
        test "eq20.mzn"


    [<Fact>]
    let ``test factory_planning_instance`` () =
        test "factory_planning_instance.mzn"


    [<Fact>]
    let ``test golomb`` () =
        test "golomb.mzn"


    [<Fact>]
    let ``test halfreif`` () =
        test "halfreif.mzn"


    [<Fact>]
    let ``test jobshop2x2`` () =
        test "jobshop2x2.mzn"


    [<Fact>]
    let ``test knights`` () =
        test "knights.mzn"


    [<Fact>]
    let ``test langford`` () =
        test "langford.mzn"


    [<Fact>]
    let ``test langford2`` () =
        test "langford2.mzn"


    [<Fact>]
    let ``test latin_squares_fd`` () =
        test "latin_squares_fd.mzn"


    [<Fact>]
    let ``test magicsq_3`` () =
        test "magicsq_3.mzn"


    [<Fact>]
    let ``test magicsq_4`` () =
        test "magicsq_4.mzn"


    [<Fact>]
    let ``test magicsq_5`` () =
        test "magicsq_5.mzn"


    [<Fact>]
    let ``test multidimknapsack_simple`` () =
        test "multidimknapsack_simple.mzn"


    [<Fact>]
    let ``test oss`` () =
        test "oss.mzn"


    [<Fact>]
    let ``test packing`` () =
        test "packing.mzn"


    [<Fact>]
    let ``test perfsq`` () =
        test "perfsq.mzn"


    [<Fact>]
    let ``test perfsq2`` () =
        test "perfsq2.mzn"


    [<Fact>]
    let ``test photo`` () =
        test "photo.mzn"


    [<Fact>]
    let ``test product_fd`` () =
        test "product_fd.mzn"


    [<Fact>]
    let ``test product_lp`` () =
        test "product_lp.mzn"


    [<Fact>]
    let ``test quasigroup_qg5`` () =
        test "quasigroup_qg5.mzn"


    [<Fact>]
    let ``test queen_cp2`` () =
        test "queen_cp2.mzn"


    [<Fact>]
    let ``test queen_ip`` () =
        test "queen_ip.mzn"


    [<Fact>]
    let ``test radiation`` () =
        test "radiation.mzn"


    [<Fact>]
    let ``test simple_sat`` () =
        test "simple_sat.mzn"


    [<Fact>]
    let ``test singHoist2`` () =
        test "singHoist2.mzn"


    [<Fact>]
    let ``test steiner-triples`` () =
        test "steiner-triples.mzn"


    [<Fact>]
    let ``test sudoku`` () =
        test "sudoku.mzn"


    [<Fact>]
    let ``test template_design`` () =
        test "template_design.mzn"


    [<Fact>]
    let ``test tenpenki_1`` () =
        test "tenpenki_1.mzn"


    [<Fact>]
    let ``test tenpenki_2`` () =
        test "tenpenki_2.mzn"


    [<Fact>]
    let ``test tenpenki_3`` () =
        test "tenpenki_3.mzn"


    [<Fact>]
    let ``test tenpenki_4`` () =
        test "tenpenki_4.mzn"


    [<Fact>]
    let ``test tenpenki_5`` () =
        test "tenpenki_5.mzn"


    [<Fact>]
    let ``test tenpenki_6`` () =
        test "tenpenki_6.mzn"


    [<Fact>]
    let ``test timetabling`` () =
        test "timetabling.mzn"


    [<Fact>]
    let ``test trucking`` () =
        test "trucking.mzn"


    [<Fact>]
    let ``test warehouses`` () =
        test "warehouses.mzn"


    [<Fact>]
    let ``test wolf_goat_cabbage`` () =
        test "wolf_goat_cabbage.mzn"


    [<Fact>]
    let ``test zebra`` () =
        test "zebra.mzn"
