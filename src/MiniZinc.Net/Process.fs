namespace MiniZinc.Net

// open System
// open System.IO
// open System.Diagnostics
// open System.Reactive.Subjects
// open System.Threading.Tasks
// open FSharp.Control.Reactive
// open System.Reactive
// open System.Text
// open System.Reactive.Linq
//
//
// type Proc =
//
//     static member create (command: string) =
//         let psi = ProcessStartInfo()
//         psi.FileName <- command
//         psi.RedirectStandardOutput <- true
//         psi.RedirectStandardError <- true
//         psi.UseShellExecute <- false
//         psi.CreateNoWindow <- true
//         psi.StandardOutputEncoding <- Encoding.UTF8
//         psi.StandardErrorEncoding <- Encoding.UTF8
//         psi 
//
//     static member create(command:string, args: string) =
//         let psi = Proc.create(command)
//         psi.Arguments <- args
//         psi
//
//     static member create (command: string, [<ParamArray>] args: Object[]) : ProcessStartInfo =
//         let psi = Proc.create(command)
//         for arg in args do
//             psi.ArgumentList.Add (string arg)
//         psi
//     
//     static member exec (psi: ProcessStartInfo) : IConnectableObservable<string> =
//     
//         use proc = new Process()
//         proc.EnableRaisingEvents <- true
//         proc.StartInfo <- psi
//         
//         let output = proc.OutputDataReceived
//         let error = proc.ErrorDataReceived
//         let exited = proc.Exited
//         let stream = 
//             [| output.AsObservable() ;
//               error.AsObservable() |]
//             |> Observable.mergeArray
//             |> Observable.map (fun data -> data.Data)
//             |> Observable.takeUntilOther exited
//             |> Observable.replay
//
//         proc.Start()
//         proc.BeginErrorReadLine()
//         proc.BeginOutputReadLine()
//         stream
//         // stream
//         
