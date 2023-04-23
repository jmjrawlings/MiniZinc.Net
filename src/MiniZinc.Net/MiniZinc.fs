namespace MiniZinc.Net

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.Json.Serialization
open MiniZinc.Net
open System.Text.Json
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Threading.Tasks
open System.Threading.Channels
        
[<Struct>]
type StartMessage =
    { process_id : int 
    ; timestamp : DateTimeOffset }
    
[<Struct>]
type ExitMessage =
    { exit_code : int
    ; is_error : bool
    ; timestamp : DateTimeOffset }
    
    
[<Struct>]
type OutputMessage =
    { text: string 
    ; timestamp : DateTimeOffset }
    
    static member make text =
        { text = text
        ; timestamp = DateTimeOffset.Now }

    static member make(text, content) =
        OutputMessage.make(text, DateTimeOffset.Now, content)
        
    static member make(text, timestamp, content) =
        { text = text
        ; timestamp = timestamp
        ; content = content }        
        
    static member map f (msg: OutputMessage<'t>) =
        OutputMessage.make(msg.text, f msg.content, msg.timestamp)
        
    static member withData data msg =
        OutputMessage.make(msg.text, msg.timestamp, data)

    
and [<Struct>]
OutputMessage<'t> =
    { text: string
    ; content : 't
    ; timestamp : DateTimeOffset }
        
    
        
[<Struct>]
type CommandMessage =
    | Started of start:StartMessage
    | Output  of output:OutputMessage
    | Error   of error:OutputMessage
    | Exited  of exit:ExitMessage
    
[<Struct>]
type CommandResult =
    { start_time: DateTimeOffset
    ; end_time : DateTimeOffset
    ; duration : TimeSpan
    ; stdout : string
    ; stderr : string
    ; exit_code : int
    ; is_error : bool }

type MiniZinc() =
    
    static let makeProcess args =
        let proc = new Process()
        proc.StartInfo.FileName <- MiniZinc.ExecutablePath
        proc.StartInfo.Arguments <- args
        proc.StartInfo.RedirectStandardOutput <- true
        proc.StartInfo.RedirectStandardError <- true
        proc.StartInfo.UseShellExecute <- false
        proc.StartInfo.CreateNoWindow <- true
        proc.EnableRaisingEvents <- true
        proc
        
    static let mutable executablePath =
        "minizinc"
      
    static member ExecutablePath
        with get() =
            executablePath
        and set value =
            executablePath <- value
    
    /// <summary>
    /// Execute a MiniZinc command 
    /// </summary>
    static member Exec (args: string) =
        use proc = makeProcess args
        let stdout = StringBuilder()
        let stderr = StringBuilder()
        let complete = TaskCompletionSource<bool>()
        
        let handleData (builder: StringBuilder) (args: DataReceivedEventArgs) =
            if args.Data <> null then
                do builder.Append args.Data
        
        proc.OutputDataReceived.Add (handleData stdout)
        proc.ErrorDataReceived.Add (handleData stderr)
        proc.Exited.Add (fun _ -> complete.SetResult true)
                
        task {
            let start_time = DateTimeOffset.Now
            
            proc.Start()
            proc.BeginOutputReadLine()
            proc.BeginErrorReadLine()
            proc.WaitForExit()
                    
            let! _ = complete.Task
            let end_time = DateTimeOffset.Now
            
            let result =
                { start_time = start_time
                ; end_time = end_time
                ; duration = end_time - start_time
                ; exit_code = proc.ExitCode
                ; is_error = proc.ExitCode > 0 
                ; stdout = string stdout
                ; stderr = string stderr }

            proc.Dispose()
            return result
        }
                
    static member Stream (args: string) =
        let proc = makeProcess args        
        let channel =
            Channel.CreateUnbounded<CommandMessage>()
            
        let handleData messageType (args: DataReceivedEventArgs) =
            match args.Data with
            | null -> ()
            | text ->
                let message : OutputMessage =
                    OutputMessage.make(text)

                do
                    message
                    |> messageType
                    |> channel.Writer.TryWrite 
                
        let handleExit _ =
            let message =
                CommandMessage.Exited {
                    exit_code = proc.ExitCode
                    is_error = proc.ExitCode > 0
                    timestamp = DateTimeOffset.Now  
                }
            proc.Dispose()
            channel.Writer.TryWrite message
            channel.Writer.Complete()

        proc.OutputDataReceived.Add (handleData CommandMessage.Output)
        proc.ErrorDataReceived.Add (handleData CommandMessage.Error)
        proc.Exited.Add handleExit
        proc.Start()
        proc.BeginOutputReadLine()
        proc.BeginErrorReadLine()
        channel.Reader.ReadAllAsync()
        
        
    /// <summary>
    /// Get all installed solvers
    /// </summary>
    static member Solvers () =
        task {
            let! result =
                MiniZinc.Exec "--solvers-json"
            
            let options =
                let opts = JsonSerializerOptions()
                opts.PropertyNameCaseInsensitive <- true
                opts

            let solvers =
                JsonSerializer.Deserialize<List<Solver>>(result.stdout, options)
                |> Map.withKey (fun s -> s.Id)
            
            return solvers
        }

    /// <summary>
    /// Find a solver by Id
    /// </summary>
    static member GetSolver id =
        MiniZinc.Solvers ()
        |> Task.map (Map.tryFind id)
    
    /// <summary>
    /// Get the installed MiniZinc version
    /// </summary>
    static member Version() =
        
        let pattern =
            @"version (\d+\.\d+\.\d+)"
            
        task {
            let! result =
                MiniZinc.Exec "--version"
            let version =
                result.stdout
                |> Grep.match1 pattern
            return version
        }