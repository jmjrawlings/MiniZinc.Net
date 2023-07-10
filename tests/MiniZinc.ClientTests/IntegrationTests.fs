    namespace MiniZinc.ClientTests
    open MiniZinc.Tests
    open Xunit

    type ``No Chain Compression``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "no-chain-compression"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


    type ``No Half Reifications``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "no-half-reifications"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


    type ``No Mip Domains``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "no-mip-domains"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


    type ``Optimize 0``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "optimize-0"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


    type ``Optimize 2``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "optimize-2"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


    type ``Optimize 3``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)

        override this.Name with get() =
            "optimize-3"

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``2DPacking`` solver =
            let testCase = this.Suite.TestCases[0]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``alpha`` solver =
            let testCase = this.Suite.TestCases[1]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships10`` solver =
            let testCase = this.Suite.TestCases[2]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_1`` solver =
            let testCase = this.Suite.TestCases[3]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_2`` solver =
            let testCase = this.Suite.TestCases[4]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_3`` solver =
            let testCase = this.Suite.TestCases[5]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_4`` solver =
            let testCase = this.Suite.TestCases[6]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_5`` solver =
            let testCase = this.Suite.TestCases[7]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``battleships_7`` solver =
            let testCase = this.Suite.TestCases[8]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``battleships_9`` solver =
            let testCase = this.Suite.TestCases[9]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_1`` solver =
            let testCase = this.Suite.TestCases[10]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``blocksworld_instance_2`` solver =
            let testCase = this.Suite.TestCases[11]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``cutstock`` solver =
            let testCase = this.Suite.TestCases[12]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``eq20`` solver =
            let testCase = this.Suite.TestCases[13]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``factory_planning_instance`` solver =
            let testCase = this.Suite.TestCases[14]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``golomb`` solver =
            let testCase = this.Suite.TestCases[15]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``halfreif`` solver =
            let testCase = this.Suite.TestCases[16]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``jobshop2x2`` solver =
            let testCase = this.Suite.TestCases[17]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``knights`` solver =
            let testCase = this.Suite.TestCases[18]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford`` solver =
            let testCase = this.Suite.TestCases[19]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``langford2`` solver =
            let testCase = this.Suite.TestCases[20]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``latin_squares_fd`` solver =
            let testCase = this.Suite.TestCases[21]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``magicsq_3`` solver =
            let testCase = this.Suite.TestCases[22]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_4`` solver =
            let testCase = this.Suite.TestCases[23]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``magicsq_5`` solver =
            let testCase = this.Suite.TestCases[24]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``multidimknapsack_simple`` solver =
            let testCase = this.Suite.TestCases[25]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("cbc")>]
        [<InlineData("chuffed")>]
        member this.``oss`` solver =
            let testCase = this.Suite.TestCases[26]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``packing`` solver =
            let testCase = this.Suite.TestCases[27]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq`` solver =
            let testCase = this.Suite.TestCases[28]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``perfsq2`` solver =
            let testCase = this.Suite.TestCases[29]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``photo`` solver =
            let testCase = this.Suite.TestCases[30]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_fd`` solver =
            let testCase = this.Suite.TestCases[31]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``product_lp`` solver =
            let testCase = this.Suite.TestCases[32]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``quasigroup_qg5`` solver =
            let testCase = this.Suite.TestCases[33]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_cp2`` solver =
            let testCase = this.Suite.TestCases[34]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``queen_ip`` solver =
            let testCase = this.Suite.TestCases[35]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``radiation`` solver =
            let testCase = this.Suite.TestCases[36]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``radiation 2`` solver =
            let testCase = this.Suite.TestCases[37]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``simple_sat`` solver =
            let testCase = this.Suite.TestCases[38]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``singHoist2`` solver =
            let testCase = this.Suite.TestCases[39]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``steiner-triples`` solver =
            let testCase = this.Suite.TestCases[40]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``sudoku`` solver =
            let testCase = this.Suite.TestCases[41]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``template_design`` solver =
            let testCase = this.Suite.TestCases[42]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_1`` solver =
            let testCase = this.Suite.TestCases[43]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_2`` solver =
            let testCase = this.Suite.TestCases[44]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_3`` solver =
            let testCase = this.Suite.TestCases[45]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_4`` solver =
            let testCase = this.Suite.TestCases[46]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_5`` solver =
            let testCase = this.Suite.TestCases[47]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``tenpenki_6`` solver =
            let testCase = this.Suite.TestCases[48]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``timetabling`` solver =
            let testCase = this.Suite.TestCases[49]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``trucking`` solver =
            let testCase = this.Suite.TestCases[50]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``warehouses`` solver =
            let testCase = this.Suite.TestCases[51]
            this.test testCase solver

        [<Theory>]
        [<InlineData("cbc")>]
        member this.``wolf_goat_cabbage`` solver =
            let testCase = this.Suite.TestCases[52]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        [<InlineData("chuffed")>]
        member this.``wolf_goat_cabbage 2`` solver =
            let testCase = this.Suite.TestCases[53]
            this.test testCase solver

        [<Theory>]
        [<InlineData("gecode")>]
        member this.``zebra`` solver =
            let testCase = this.Suite.TestCases[54]
            this.test testCase solver


