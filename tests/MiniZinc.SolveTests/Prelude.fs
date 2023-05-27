namespace MiniZinc.Solve.Tests

open System.Runtime.CompilerServices

[<AutoOpen>]
module Prelude =

    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member inline AssertEquals(a, b) =
            if a <> b then
                failwithf $"{a} does not equal {b}"