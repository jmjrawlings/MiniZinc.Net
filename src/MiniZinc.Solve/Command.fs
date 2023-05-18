(*
Command.fs
*)

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
open System.Collections
open System.Threading.Channels
open System.Text.RegularExpressions


module rec Command =
                
    [<Struct>]
    type StartMessage =
        { ProcessId : int 
        ; TimeStamp : DateTimeOffset }
        
    [<Struct>]
    type ExitMessage =
        { ExitCode  : int
        ; IsError   : bool
        ; TimeStamp : DateTimeOffset }
        
    [<Struct>]
    type OutputMessage =
        { Text: string 
        ; TimeStamp : DateTimeOffset }
        
        static member make text =
            { Text = text
            ; TimeStamp = DateTimeOffset.Now }

        static member make(text, content) =
            OutputMessage.make(text, DateTimeOffset.Now, content)
            
        static member make(text, timestamp, content) =
            { Text = text
            ; TimeStamp = timestamp
            ; Content = content }        
            
        static member map f (msg: OutputMessage<'t>) =
            OutputMessage.make(msg.Text, f msg.Content, msg.TimeStamp)
            
        static member withData data msg =
            OutputMessage.make(msg.Text, msg.TimeStamp, data)
        
    and [<Struct>]
    OutputMessage<'t> =
        { Text: string
        ; Content : 't
        ; TimeStamp : DateTimeOffset }
            
    [<Struct>]
    type CommandMessage =
        | Started of start:StartMessage
        | Output  of output:OutputMessage
        | Error   of error:OutputMessage
        | Exited  of exit:ExitMessage
        
    [<Struct>]
    type CommandResult =
        { Command   : string
        ; Args      : string list
        ; Statement : string
        ; StartTime : DateTimeOffset
        ; EndTime   : DateTimeOffset
        ; Duration  : TimeSpan
        ; StdOut    : string
        ; StdErr    : string
        ; ExitCode  : int
        ; IsError   : bool }

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
            
        override this.ToString() =
            match this with
            | FlagOnly f -> f
            | FlagAndValue (f,s,v) -> $"{f}{s}{v}"
            | ValueOnly v -> v
                
    module Arg =

        // Parse the given string as an Arg
        let parse (s: string) : Arg =
            let mutable value = ""
            let mutable sep = ""
            let mutable flag = ""
            
            let quoted_pattern = "\"[^\"]*\""
            let unquoted_pattern = "[^\s\"<>|&;]*"
            let flag_pattern = "-(?:-?[\w|-]+)"
            let assign_pattern = $"({flag_pattern})(=|\s)?(.*)"
            let assign_regex = Regex assign_pattern
            let value_pattern = $"({quoted_pattern})|({unquoted_pattern})|(.*)"
            let value_regex = Regex value_pattern
            let assign_match = assign_regex.Match s 
            if assign_match.Success then
                flag <- assign_match.Groups[1].Value
                sep <- assign_match.Groups[2].Value
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
        internal
        | Args of Arg list

        member this.List =
            match this with
            | Args list -> list
            
        override this.ToString() =
            Args.toString this
            
        interface IEnumerable<Arg> with
            member this.GetEnumerator() =
                (this.List :> IEnumerable<Arg>).GetEnumerator()

            member this.GetEnumerator() =
                (this.List :> IEnumerable).GetEnumerator()
                
        static member Create([<ParamArray>] args: obj[]) =
            args
            |> Seq.map string
            |> Args.parseMany
            
        member this.Append(arg: Arg) = 
            match this with
            | Args xs -> Args (xs @ [arg])
        
        member this.Append(Args args: Args) = 
            match this with
            | Args xs -> Args (xs @ args)
            
        member this.Append([<ParamArray>] args: obj[]) =
            let args' =
                args
                |> Seq.map string
                |> Args.parseMany
            
            (this.Append: Args -> Args)(args')
            
        static member (+) (a: Args, b: Args) =
            match a,b with
            | Args a', Args b' -> Args (a' @ b')
            
        static member (+) (a: Args, b: string) =
            a + (Args.parse b)
            
        static member (+) (a: Args, b: Arg) =
            a + (Args [b])
    
        
    module Args =
        
        // Empty argument list
        let empty =
            Args []
        
        // Apply a function to the arguments
        let map f args =
            match args with
            | Args args -> Args (f args)
        
        // Add an argument
        let append arg args =
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
            
        let parseMany (strings: string seq) =
            strings
            |> Seq.map parse
            |> Seq.collect toList
            |> Seq.toList
            |> Args
            
        let ofSeq (args: Arg seq) =
            args
            |> Seq.toList
            |> Args
            
        let ofList (args: Arg list) =
            Args args


    type Command =
        { Exe   : string
          Args  : Args }
        
        static member Create(exe: string, [<ParamArray>] args: obj[]) =
            let args = Args.Create(args)
            { Exe = exe; Args = args; }
            
        member this.AddArgs([<ParamArray>] args: obj[]) =
            { this with Args = this.Args.Append(args) }
            
        member this.Statement =
            Command.statement this
            
        member this.Exec() =
            Command.exec this
            
        member this.Stream() =
            Command.stream this
    
        
    module Command =
    
        let empty =
            { Exe = ""; Args = Args.empty; }
        
        let withArgs (args: Args) (cmd: Command) =
            { cmd with Args = args }
        
        /// <summary>
        /// Return the full command line statement of
        /// a command
        /// </summary>    
        let statement (cmd: Command) =
            let statement = $"{cmd.Exe} {cmd.Args}"  
            statement

        /// <summary>
        /// Create a System.Diagnostics.Process representing
        /// the exe and arguments of the given command
        /// </summary>
        let toProcess (cmd: Command) =
            let proc = new Process()
            proc.StartInfo.FileName <- cmd.Exe
            proc.StartInfo.RedirectStandardOutput <- true
            proc.StartInfo.RedirectStandardError <- true
            proc.StartInfo.UseShellExecute <- false
            proc.StartInfo.CreateNoWindow <- true
            proc.EnableRaisingEvents <- true
            for arg in cmd.Args do
                proc.StartInfo.ArgumentList.Add (string arg)
            proc
            
        /// <summary>
        /// Execute the given command and listen asynchronously
        /// for messages.
        /// </summary>
        let stream (cmd: Command) : IAsyncEnumerable<CommandMessage> =
            
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
                        ExitCode = proc.ExitCode
                        IsError = proc.ExitCode > 0
                        TimeStamp = DateTimeOffset.Now  
                    }
                proc.Dispose()
                channel.Writer.TryWrite message
                channel.Writer.Complete()

            proc.OutputDataReceived.Add (handleData CommandMessage.Output)
            proc.ErrorDataReceived.Add (handleData CommandMessage.Error)
            proc.Exited.Add handleExit
            proc.Start()

            { ProcessId = proc.Id
            ; TimeStamp = DateTimeOffset.Now }
            |> CommandMessage.Started
            |> channel.Writer.TryWrite
            
            proc.BeginOutputReadLine()
            proc.BeginErrorReadLine()
            channel.Reader.ReadAllAsync()
        
                    
        /// <summary>
        /// Execute a Command and return full output from
        /// stdout and stderr
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
                    { Command = proc.StartInfo.FileName
                    ; Args = Seq.toList proc.StartInfo.ArgumentList
                    ; Statement = statement 
                    ; StartTime = start_time
                    ; EndTime = end_time
                    ; Duration = end_time - start_time
                    ; ExitCode = proc.ExitCode
                    ; IsError = proc.ExitCode > 0 
                    ; StdOut = string stdout
                    ; StdErr = string stderr }

                proc.Dispose()
                return result
            }