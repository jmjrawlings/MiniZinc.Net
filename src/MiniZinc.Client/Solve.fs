namespace MiniZinc

open System.Collections.Generic
open System.Text.Json.Nodes

/// An installed MiniZinc solver
type Solver = {
    Id                  : string
    Name                : string
    Version             : string
    MznLib              : string
    Executable          : string
    Tags                : IReadOnlyList<string>
    SupportsMzn         : bool
    SupportsFzn         : bool
    SupportsNL          : bool
    NeedsSolns2Out      : bool 
    NeedsMznExecutable  : bool
    NeedsStdlibDir      : bool
    NeedsPathsFile      : bool
    IsGUIApplication    : bool
    MznLibVersion       : int
    ExtraInfo           : JsonObject
    Description         : string
    StdFlags            : IReadOnlyList<string>    
    RequiredFlags       : IReadOnlyList<string>    
    ExtraFlags          : IReadOnlyList<IReadOnlyList<string>>
}