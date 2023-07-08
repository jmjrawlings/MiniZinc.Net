namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module ``Integration Tests`` =

    let cwd =
        Directory.GetCurrentDirectory()
        |> DirectoryInfo
        
    let projectDir =
        cwd.Parent.Parent.Parent.Parent.Parent
        
    let libminizincDir =
        projectDir <//> "tests" <//> "libminizinc"
       
    type IntegrationTestSuite(fixture: ClientFixture) =
    
        let client = fixture.Client
                
        let fail msg =
            Assert.Fail(msg)
            failwith ""
            
        interface IClassFixture<ClientFixture>            
            
        member _.test filePath solver =
                
            let file =
                libminizincDir </> filePath
            
            let options =
                SolveOptions.create solver
                
            let model =
                match parseModelFile file.FullName with
                | Result.Ok model ->
                    model
                | Result.Error err ->
                    fail err.Message
                    
            let solution =
                client.SolveSync(model, options)
                
            ()


            
    type ``No Chain Compression``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver


            
    type ``No Half Reifications``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver


            
    type ``No Mip Domains``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver


            
    type ``Optimize 0``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver


            
    type ``Optimize 2``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver


            
    type ``Optimize 3``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)
                
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``2DPacking`` solver =
                this.test @"examples\2DPacking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``alpha`` solver =
                this.test @"examples\alpha.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships10`` solver =
                this.test @"examples\battleships10.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_1`` solver =
                this.test @"examples\battleships_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_2`` solver =
                this.test @"examples\battleships_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_3`` solver =
                this.test @"examples\battleships_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_4`` solver =
                this.test @"examples\battleships_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_5`` solver =
                this.test @"examples\battleships_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``battleships_7`` solver =
                this.test @"examples\battleships_7.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``battleships_9`` solver =
                this.test @"examples\battleships_9.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_1`` solver =
                this.test @"examples\blocksworld_instance_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``blocksworld_instance_2`` solver =
                this.test @"examples\blocksworld_instance_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``cutstock`` solver =
                this.test @"examples\cutstock.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``eq20`` solver =
                this.test @"examples\eq20.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``factory_planning_instance`` solver =
                this.test @"examples\factory_planning_instance.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``golomb`` solver =
                this.test @"examples\golomb.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``halfreif`` solver =
                this.test @"examples\halfreif.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``jobshop2x2`` solver =
                this.test @"examples\jobshop2x2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``knights`` solver =
                this.test @"examples\knights.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford`` solver =
                this.test @"examples\langford.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``langford2`` solver =
                this.test @"examples\langford2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``latin_squares_fd`` solver =
                this.test @"examples\latin_squares_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``magicsq_3`` solver =
                this.test @"examples\magicsq_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_4`` solver =
                this.test @"examples\magicsq_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``magicsq_5`` solver =
                this.test @"examples\magicsq_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``multidimknapsack_simple`` solver =
                this.test @"examples\multidimknapsack_simple.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("cbc")>]
            [<InlineData("chuffed")>]
            member this.``oss`` solver =
                this.test @"examples\oss.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``packing`` solver =
                this.test @"examples\packing.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq`` solver =
                this.test @"examples\perfsq.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``perfsq2`` solver =
                this.test @"examples\perfsq2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``photo`` solver =
                this.test @"examples\photo.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_fd`` solver =
                this.test @"examples\product_fd.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``product_lp`` solver =
                this.test @"examples\product_lp.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``quasigroup_qg5`` solver =
                this.test @"examples\quasigroup_qg5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_cp2`` solver =
                this.test @"examples\queen_cp2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``queen_ip`` solver =
                this.test @"examples\queen_ip.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``radiation`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``radiation 2`` solver =
                this.test @"examples\radiation.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``simple_sat`` solver =
                this.test @"examples\simple_sat.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``singHoist2`` solver =
                this.test @"examples\singHoist2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``steiner-triples`` solver =
                this.test @"examples\steiner-triples.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``sudoku`` solver =
                this.test @"examples\sudoku.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``template_design`` solver =
                this.test @"examples\template_design.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_1`` solver =
                this.test @"examples\tenpenki_1.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_2`` solver =
                this.test @"examples\tenpenki_2.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_3`` solver =
                this.test @"examples\tenpenki_3.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_4`` solver =
                this.test @"examples\tenpenki_4.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_5`` solver =
                this.test @"examples\tenpenki_5.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``tenpenki_6`` solver =
                this.test @"examples\tenpenki_6.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``timetabling`` solver =
                this.test @"examples\timetabling.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``trucking`` solver =
                this.test @"examples\trucking.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``warehouses`` solver =
                this.test @"examples\warehouses.mzn" solver
            
            [<Theory>]
            [<InlineData("cbc")>]
            member this.``wolf_goat_cabbage`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            [<InlineData("chuffed")>]
            member this.``wolf_goat_cabbage 2`` solver =
                this.test @"examples\wolf_goat_cabbage.mzn" solver
            
            [<Theory>]
            [<InlineData("gecode")>]
            member this.``zebra`` solver =
                this.test @"examples\zebra.mzn" solver
