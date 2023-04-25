﻿namespace MiniZinc

open System
open System.Diagnostics
open System.IO
open System.Text
open MiniZinc
open System.Text.Json
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Threading.Channels

module rec Command =
                
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
        { command    : string
        ; args       : string[]
        ; statement  : string
        ; start_time : DateTimeOffset
        ; end_time   : DateTimeOffset
        ; duration   : TimeSpan
        ; stdout     : string
        ; stderr     : string
        ; exit_code  : int
        ; is_error   : bool }
    

    type Command =
        { exe: string
          args : string list }
        
        static member Create (cmd: string) =
            let proc = new Process()
            proc.StartInfo.FileName <- cmd
            proc.StartInfo.RedirectStandardOutput <- true
            proc.StartInfo.RedirectStandardError <- true
            proc.StartInfo.UseShellExecute <- false
            proc.StartInfo.CreateNoWindow <- true
            proc.EnableRaisingEvents <- true
            proc
            
        static member Create(cmd: string, [<ParamArray>] args: string[]) =
            let command = Command.Create(cmd)
            for arg in args do
                let escaped = Arg.escape(arg)
                command.StartInfo.ArgumentList.Add escaped
            command
            
        static member Create(cmd: string, args: string seq) =
            let args = Seq.toArray args
            let command = Command.Create(cmd, args)
            command
        
        /// <summary>
        /// Execute a Command 
        /// </summary>    
        static member Exec(proc: Process) =
            let statement = Command.statement proc.StartInfo
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
                    { command = proc.StartInfo.FileName
                    ; args = Seq.toArray proc.StartInfo.ArgumentList
                    ; statement = statement 
                    ; start_time = start_time
                    ; end_time = end_time
                    ; duration = end_time - start_time
                    ; exit_code = proc.ExitCode
                    ; is_error = proc.ExitCode > 0 
                    ; stdout = string stdout
                    ; stderr = string stderr }

                proc.Dispose()
                return result
            }
            
        static member Exec(cmd: string, [<ParamArray>] args: string[]) =
            let proc = Command.Create(cmd, args)
            Command.Exec(proc)
            
        static member Exec(cmd: string, args: string seq) =
            let proc = Command.Create(cmd, args)
            Command.Exec(proc)
                    
        static member Stream (proc: Process) =
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
        
        static member Stream(cmd: string, [<ParamArray>] args: string[]) =
            let proc = Command.Create(cmd, args)
            Command.Stream(proc)
            
        static member Stream(cmd: string, args: string seq) =
            let proc = Command.Create(cmd, args)
            Command.Stream(proc)

    [<RequireQualifiedAccess>]
    type FlagType = | Short | Long | None

    type Arg =
        | Flag of string
        | Assign of string*string
        | Value of string
                
    module Arg =
        open System.Text.RegularExpressions
        let pattern =
             Regex("(--|-|/)(\w+)(=|:)?(.+)?")
        let parse (s: string) =
            match pattern.Match(s) with
            | m when m.Success ->
                let prefix = m.Groups.[1].Value
                let key = m.Groups.[2].Value
                let flag = $"{prefix}{key}"
                if m.Groups.[4].Success then
                    let value = m.Groups.[4].Value
                    Some <| Arg.Assign(flag, value)
                else
                    Some <| Arg.Flag flag
            | _ ->
                Some (Arg.Value s)
                
        let escape (arg: string) =
            if arg.StartsWith("-") then
                arg
            elif arg.Contains('\"') then
                let escaped = arg.Replace("\"", "\\\"")
                $"\"{escaped}\""
            else
                arg
                

    
    module Command =

        let create (exe: string) =
            { exe = exe
            ; args = [] }
        
        let withArgs (args: string seq) (cmd: Command) =
            { cmd with args = Seq.toList args }
            
        let addArg (arg: string) (cmd: Command) =
            { cmd with args = cmd.args @ [arg] }
        let statement (proc: ProcessStartInfo) =
            let statement = $"{proc.FileName} " + String.concat " " proc.ArgumentList
            statement
