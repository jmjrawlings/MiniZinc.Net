namespace MiniZinc.Solve.Tests

open System.Runtime.CompilerServices
open FluentAssertions

[<AutoOpen>]
module Prelude =

    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member ShouldEqual(a: obj, b: string) =
            (string a).Should().Be(b, "")