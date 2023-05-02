namespace MiniZinc.Net.Tests

open System.Runtime.CompilerServices
open FluentAssertions

[<Extension>]
type Prelude() =
    
    [<Extension>]
    static member ShouldEqual(a: obj, b: string) =
        (string a).Should().Be(b, "")
