namespace MiniZinc.Net.Tests

module ExpectoTemplate =

    open Expecto

    [<EntryPoint>]
    let main argv =
        Tests.runTestsInAssembly defaultConfig argv
