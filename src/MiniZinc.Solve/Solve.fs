namespace MiniZinc

open System
open System.Text.Json.Nodes
open System.Text.Json.Serialization

/// <summary>
/// An installed MiniZinc solver
/// </summary>
type Solver = {
    Id : string
    Name : string
    Version : string
    MznLib: string
    Executable : string
    Tags : string list
    SupportsMzn: bool
    SupportsFzn: bool
    SupportsNL: bool
    NeedsSolns2Out: bool 
    NeedsMznExecutable: bool
    NeedsStdlibDir: bool
    NeedsPathsFile: bool
    IsGUIApplication: bool
    MznLibVersion: int
    ExtraInfo : JsonObject
    Description:string
    StdFlags : List<string>    
    RequiredFlags : List<string>    
    ExtraFlags: List<List<string>>
}    
