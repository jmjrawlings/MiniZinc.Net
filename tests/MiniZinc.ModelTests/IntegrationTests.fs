
namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit

module ``Integration Tests`` =
    
    let test (name: string) =
        let suite = TestSuite.load name
        let model = TestSuite.parseModel suite
        let mzn = Model.encode EncodingOptions.Default model
        let roundtrip = Model.parseExn ParseOptions.Default mzn
            
        //     
        // let a = 1
        ()
        

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
    let ``test battleships 1`` () =
        test "battleships_1.mzn"


    [<Fact>]
    let ``test battleships 2`` () =
        test "battleships_2.mzn"


    [<Fact>]
    let ``test battleships 3`` () =
        test "battleships_3.mzn"


    [<Fact>]
    let ``test battleships 4`` () =
        test "battleships_4.mzn"


    [<Fact>]
    let ``test battleships 5`` () =
        test "battleships_5.mzn"


    [<Fact>]
    let ``test battleships 7`` () =
        test "battleships_7.mzn"


    [<Fact>]
    let ``test battleships 9`` () =
        test "battleships_9.mzn"


    [<Fact>]
    let ``test blocksworld instance 1`` () =
        test "blocksworld_instance_1.mzn"


    [<Fact>]
    let ``test blocksworld instance 2`` () =
        test "blocksworld_instance_2.mzn"


    [<Fact>]
    let ``test cutstock`` () =
        test "cutstock.mzn"


    [<Fact>]
    let ``test eq20`` () =
        test "eq20.mzn"


    [<Fact>]
    let ``test factory planning instance`` () =
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
    let ``test latin squares fd`` () =
        test "latin_squares_fd.mzn"


    [<Fact>]
    let ``test magicsq 3`` () =
        test "magicsq_3.mzn"


    [<Fact>]
    let ``test magicsq 4`` () =
        test "magicsq_4.mzn"


    [<Fact>]
    let ``test magicsq 5`` () =
        test "magicsq_5.mzn"


    [<Fact>]
    let ``test multidimknapsack simple`` () =
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
    let ``test product fd`` () =
        test "product_fd.mzn"


    [<Fact>]
    let ``test product lp`` () =
        test "product_lp.mzn"


    [<Fact>]
    let ``test quasigroup qg5`` () =
        test "quasigroup_qg5.mzn"


    [<Fact>]
    let ``test queen cp2`` () =
        test "queen_cp2.mzn"


    [<Fact>]
    let ``test queen ip`` () =
        test "queen_ip.mzn"


    [<Fact>]
    let ``test radiation`` () =
        test "radiation.mzn"


    [<Fact>]
    let ``test simple sat`` () =
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
    let ``test template design`` () =
        test "template_design.mzn"


    [<Fact>]
    let ``test tenpenki 1`` () =
        test "tenpenki_1.mzn"


    [<Fact>]
    let ``test tenpenki 2`` () =
        test "tenpenki_2.mzn"


    [<Fact>]
    let ``test tenpenki 3`` () =
        test "tenpenki_3.mzn"


    [<Fact>]
    let ``test tenpenki 4`` () =
        test "tenpenki_4.mzn"


    [<Fact>]
    let ``test tenpenki 5`` () =
        test "tenpenki_5.mzn"


    [<Fact>]
    let ``test tenpenki 6`` () =
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
    let ``test wolf goat cabbage`` () =
        test "wolf_goat_cabbage.mzn"


    [<Fact>]
    let ``test zebra`` () =
        test "zebra.mzn"
