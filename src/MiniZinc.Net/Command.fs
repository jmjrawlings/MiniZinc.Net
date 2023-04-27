namespace MiniZinc

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
open System.Text.RegularExpressions


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
          args : Args }
        
        static member Create exe =
            Command.create exe Args.empty
            
        static member Create (exe, args) =
            Command.create exe args
            
        // static member Create(exe: string, [<ParamArray>] args: obj[]) =
        //     Command.create exe args                

        // static member Create(exe: string, args: string seq) =
        //     Command.create exe (Args.ofSeq args)
            
        member this.Exec() =
            Command.exec this
        
        member this.Stream() =
            Command.stream this

    [<RequireQualifiedAccess>]
    type FlagType = | Short | Long | None

    /// <summary>
    /// A command line argument
    /// </summary>
    [<Struct>]
    type Arg =
        internal
        | FlagOnly of flag:string
        | FlagAndValue of (string*string*string)
        | ValueOnly of value:string
        
        member this.Flag =
            match this with
            | FlagOnly f | FlagAndValue(f, _, _) -> f
            | _ -> ""
            
        member this.Value =
            match this with
            | FlagAndValue (_, _, v) | ValueOnly v -> v
            | _ -> ""
            
                
    module Arg =

        // Parse the given string as an Arg
        let parse (s: string) : Arg =
            let mutable value = ""
            let mutable sep = ""
            let mutable flag = ""
            let mutable result = Result.Error ""
            
            let quoted_pattern = "\"[^\"]*\""
            let unquoted_pattern = "[^\s\"<>|&;]*"
            let flag_pattern = "-(?:-?[\w|-]+)"
            let assign_pattern = $"({flag_pattern})(=|\s)?(.*)"
            let assign_regex = Regex assign_pattern
            let value_pattern = $"({quoted_pattern})|({unquoted_pattern})|(.*)"
            let value_regex = Regex value_pattern
            let assign_match = assign_regex.Match s 
            if assign_match.Success then
                flag <- assign_match.Groups.[1].Value
                sep <- assign_match.Groups.[2].Value
                value <- assign_match.Groups[3].Value
            else
                value <- s
            
            let value_match = value_regex.Match <| value.Trim()
            if value_match.Success then
                let quoted = value_match.Groups[1].Value
                let unquoted = value_match.Groups[2].Value
                let bad = value_match.Groups[3].Value
                value <-
                    match (quoted.Length, unquoted.Length, bad.Length) with
                    | n, _, _ when n > 0 ->
                        quoted
                    | _, n, _ when n > 0 ->
                        unquoted
                    | _, _, n when n > 0 ->
                        $"\"{bad}\""
                    | _ ->
                        ""
                        
            match (flag, sep, value) with
            | "", "", "" -> ValueOnly ""
            | f, "", "" -> FlagOnly f
            | "", "", v -> ValueOnly v
            | f, s, v -> FlagAndValue (f, s, v)
            
    /// <summary>
    /// Command line arguments
    /// </summary>
    type Args =
        | Args of Arg list

        override this.ToString() =
            Args.toString this
        
    module Args =
        
        // Empty argument list
        let empty =
            Args []
        
        // Apply a function to the arguments
        let map f args =
            match args with
            | Args args -> Args (f args)
        
        // Add an argument
        let add arg args =
            map (fun args -> args @ [arg]) args
            
        // Get the list of args            
        let toList args =
            match args with
            | Args args -> args
            
        let toString args =
            args
            |> toList
            |> List.map string
            |> String.concat " "
            
        let parse (s: string) =
            let tokens =
                s.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            let args =
                tokens
                |> Seq.map Arg.parse
                |> Seq.toList
                |> Args
            args
            
        let ofSeq (strings: string seq) =
            strings
            |> Seq.map parse
            |> Seq.collect toList
            |> Seq.toList
            |> Args
    
    module Command =

        let create (exe: string) (args: Args) =
            { exe = exe; args = args }
    
        let empty = create "" Args.empty
        
        let withArgs (args: Args) (cmd: Command) =
            { cmd with args = args }
            
        let addArg (arg: Arg) (cmd: Command) =
            withArgs (Args.add arg cmd.args)
            
        let statement (cmd: Command) =
            let statement = $"{cmd.exe} {cmd.args}"  
            statement
            
        let toProcess (cmd: Command) =
            let proc = new Process()
            proc.StartInfo.FileName <- cmd.exe
            proc.StartInfo.RedirectStandardOutput <- true
            proc.StartInfo.RedirectStandardError <- true
            proc.StartInfo.UseShellExecute <- false
            proc.StartInfo.CreateNoWindow <- true
            proc.EnableRaisingEvents <- true
            proc.StartInfo.Arguments = cmd.args.ToString()
            proc


        let stream (cmd: Command) =
            
            let proc =
                toProcess cmd
                
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
        /// Execute a Command 
        /// </summary>    
        let exec (cmd: Command) =
            let proc = toProcess cmd
            let statement = Command.statement cmd
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