
namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module IntegrationTests =

    let cwd =
        Directory.GetCurrentDirectory()
        |> DirectoryInfo
        
    let projectDir =
        cwd.Parent.Parent.Parent.Parent.Parent
        
    let specDir =
        projectDir <//> "tests" <//> "libminizinc"
        
    let testParseFile filePath =
        let file = projectDir </> filePath
        let result = parseModelFile file.FullName
        match result with
        | Result.Ok model ->
            ()
        | Result.Error err ->
            failwith err.Message
            

    [<Fact>]
    let ``test 2DPacking`` () =
        testParseFile @"tests\libminizinc\examples\2DPacking.mzn"

    [<Fact>]
    let ``test alpha`` () =
        testParseFile @"tests\libminizinc\examples\alpha.mzn"

    [<Fact>]
    let ``test battleships10`` () =
        testParseFile @"tests\libminizinc\examples\battleships10.mzn"

    [<Fact>]
    let ``test battleships_1`` () =
        testParseFile @"tests\libminizinc\examples\battleships_1.mzn"

    [<Fact>]
    let ``test battleships_2`` () =
        testParseFile @"tests\libminizinc\examples\battleships_2.mzn"

    [<Fact>]
    let ``test battleships_3`` () =
        testParseFile @"tests\libminizinc\examples\battleships_3.mzn"

    [<Fact>]
    let ``test battleships_4`` () =
        testParseFile @"tests\libminizinc\examples\battleships_4.mzn"

    [<Fact>]
    let ``test battleships_5`` () =
        testParseFile @"tests\libminizinc\examples\battleships_5.mzn"

    [<Fact>]
    let ``test battleships_7`` () =
        testParseFile @"tests\libminizinc\examples\battleships_7.mzn"

    [<Fact>]
    let ``test battleships_9`` () =
        testParseFile @"tests\libminizinc\examples\battleships_9.mzn"

    [<Fact>]
    let ``test blocksworld_instance_1`` () =
        testParseFile @"tests\libminizinc\examples\blocksworld_instance_1.mzn"

    [<Fact>]
    let ``test blocksworld_instance_2`` () =
        testParseFile @"tests\libminizinc\examples\blocksworld_instance_2.mzn"

    [<Fact>]
    let ``test cutstock`` () =
        testParseFile @"tests\libminizinc\examples\cutstock.mzn"

    [<Fact>]
    let ``test eq20`` () =
        testParseFile @"tests\libminizinc\examples\eq20.mzn"

    [<Fact>]
    let ``test factory_planning_instance`` () =
        testParseFile @"tests\libminizinc\examples\factory_planning_instance.mzn"

    [<Fact>]
    let ``test golomb`` () =
        testParseFile @"tests\libminizinc\examples\golomb.mzn"

    [<Fact>]
    let ``test halfreif`` () =
        testParseFile @"tests\libminizinc\examples\halfreif.mzn"

    [<Fact>]
    let ``test jobshop2x2`` () =
        testParseFile @"tests\libminizinc\examples\jobshop2x2.mzn"

    [<Fact>]
    let ``test knights`` () =
        testParseFile @"tests\libminizinc\examples\knights.mzn"

    [<Fact>]
    let ``test langford`` () =
        testParseFile @"tests\libminizinc\examples\langford.mzn"

    [<Fact>]
    let ``test langford2`` () =
        testParseFile @"tests\libminizinc\examples\langford2.mzn"

    [<Fact>]
    let ``test latin_squares_fd`` () =
        testParseFile @"tests\libminizinc\examples\latin_squares_fd.mzn"

    [<Fact>]
    let ``test magicsq_3`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_3.mzn"

    [<Fact>]
    let ``test magicsq_4`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_4.mzn"

    [<Fact>]
    let ``test magicsq_5`` () =
        testParseFile @"tests\libminizinc\examples\magicsq_5.mzn"

    [<Fact>]
    let ``test multidimknapsack_simple`` () =
        testParseFile @"tests\libminizinc\examples\multidimknapsack_simple.mzn"

    [<Fact>]
    let ``test oss`` () =
        testParseFile @"tests\libminizinc\examples\oss.mzn"

    [<Fact>]
    let ``test packing`` () =
        testParseFile @"tests\libminizinc\examples\packing.mzn"

    [<Fact>]
    let ``test perfsq`` () =
        testParseFile @"tests\libminizinc\examples\perfsq.mzn"

    [<Fact>]
    let ``test perfsq2`` () =
        testParseFile @"tests\libminizinc\examples\perfsq2.mzn"

    [<Fact>]
    let ``test photo`` () =
        testParseFile @"tests\libminizinc\examples\photo.mzn"

    [<Fact>]
    let ``test product_fd`` () =
        testParseFile @"tests\libminizinc\examples\product_fd.mzn"

    [<Fact>]
    let ``test product_lp`` () =
        testParseFile @"tests\libminizinc\examples\product_lp.mzn"

    [<Fact>]
    let ``test quasigroup_qg5`` () =
        testParseFile @"tests\libminizinc\examples\quasigroup_qg5.mzn"

    [<Fact>]
    let ``test queen_cp2`` () =
        testParseFile @"tests\libminizinc\examples\queen_cp2.mzn"

    [<Fact>]
    let ``test queen_ip`` () =
        testParseFile @"tests\libminizinc\examples\queen_ip.mzn"

    [<Fact>]
    let ``test radiation`` () =
        testParseFile @"tests\libminizinc\examples\radiation.mzn"

    [<Fact>]
    let ``test simple_sat`` () =
        testParseFile @"tests\libminizinc\examples\simple_sat.mzn"

    [<Fact>]
    let ``test singHoist2`` () =
        testParseFile @"tests\libminizinc\examples\singHoist2.mzn"

    [<Fact>]
    let ``test steiner-triples`` () =
        testParseFile @"tests\libminizinc\examples\steiner-triples.mzn"

    [<Fact>]
    let ``test sudoku`` () =
        testParseFile @"tests\libminizinc\examples\sudoku.mzn"

    [<Fact>]
    let ``test template_design`` () =
        testParseFile @"tests\libminizinc\examples\template_design.mzn"

    [<Fact>]
    let ``test tenpenki_1`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_1.mzn"

    [<Fact>]
    let ``test tenpenki_2`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_2.mzn"

    [<Fact>]
    let ``test tenpenki_3`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_3.mzn"

    [<Fact>]
    let ``test tenpenki_4`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_4.mzn"

    [<Fact>]
    let ``test tenpenki_5`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_5.mzn"

    [<Fact>]
    let ``test tenpenki_6`` () =
        testParseFile @"tests\libminizinc\examples\tenpenki_6.mzn"

    [<Fact>]
    let ``test timetabling`` () =
        testParseFile @"tests\libminizinc\examples\timetabling.mzn"

    [<Fact>]
    let ``test trucking`` () =
        testParseFile @"tests\libminizinc\examples\trucking.mzn"

    [<Fact>]
    let ``test warehouses`` () =
        testParseFile @"tests\libminizinc\examples\warehouses.mzn"

    [<Fact>]
    let ``test wolf_goat_cabbage`` () =
        testParseFile @"tests\libminizinc\examples\wolf_goat_cabbage.mzn"

    [<Fact>]
    let ``test zebra`` () =
        testParseFile @"tests\libminizinc\examples\zebra.mzn"
