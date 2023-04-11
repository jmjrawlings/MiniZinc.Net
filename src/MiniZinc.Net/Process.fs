namespace MiniZinc.Net

open System
open System.IO
open System.Diagnostics
open System.Threading.Tasks
open FSharp.Control

#nowarn "0020"

module rec Process =

    let exec (command: string) (args: seq<'t>) =
        let startInfo = ProcessStartInfo()
        startInfo.FileName <- command

        for a in args do
            startInfo.ArgumentList.Add(string a)
            
        startInfo.RedirectStandardOutput <- true
        startInfo.RedirectStandardError <- true
        startInfo.UseShellExecute <- false
        startInfo.CreateNoWindow <- true

        use proc = new Process()
        proc.StartInfo <- startInfo
        proc.EnableRaisingEvents <- true
        proc.Start() 

        asyncSeq {
            while true do
                let! data_args = Async.AwaitEvent proc.OutputDataReceived
                let! err_args = Async.AwaitEvent proc.ErrorDataReceived
                yield data_args
        }
