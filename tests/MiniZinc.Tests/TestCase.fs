namespace MiniZinc.Tests

open System
open System.IO
open System.Reflection
open System.Text

type TestCase =
    { Name : string
    ; FileName: string
    ; Mzn : string }

module TestCase =
    
    let assembly =
        typeof<TestCase>.Assembly
    
    let load (name: string) =
        let resourceName = $"CoreTests.{name}.mzn"
        use stream = assembly.GetManifestResourceStream resourceName 
        use reader = new StreamReader(stream)
        reader.ReadLine()
                
        let mutable stop = false
        let spec = StringBuilder()
                
        while (not stop) do
            let line = reader.ReadLine()
            if line = "***/" then
                stop <- true
            else
                spec.Append line
                ()
                
        let mzn = reader.ReadToEnd()
        let case =
            { Name = name
            ; FileName = resourceName
            ; Mzn = mzn }
        
        case