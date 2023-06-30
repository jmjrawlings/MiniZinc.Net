(*
Command.fs
*)

namespace MiniZinc

open System
open System.Diagnostics
open System.Text
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Collections
open System.Threading.Channels
open System.Text.RegularExpressions


module rec Command =
        
    type CommandStatus =
        | Started = 0
        | StdOut = 1
        | StdErr = 2
        | Success = 3
        | Failure = 4
                    
    [<Struct>]
    type CommandMessage =
        { ProcessId : int
        ; StartTime : DateTimeOffset
        ; TimeStamp : DateTimeOffset
        ; Elapsed   : TimeSpan
        ; Message   : string
        ; Status    : CommandStatus }
        
    type CommandResult =
        { Command   : string
        ; Args      : string seq
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
    
    /// A command line argument
    type Arg =
        internal
        | FlagOnly of string
        | FlagAndValue of (string*string*string)
        | ValueOnly of string
        
        member this.Flag =
            Arg.flag this
            
        member this.Value =
            Arg.value this
            
        override this.ToString() =
            Arg.toString this
           
    /// A command line argument
    module Arg =

        let flag arg  =
            match arg with
            | FlagOnly f | FlagAndValue(f, _, _) -> f
            | _ -> ""
            
        let value arg =
            match arg with
            | FlagAndValue (_, _, v) | ValueOnly v -> v
            | _ -> ""

        let toString arg =
            match arg with
            | FlagOnly f -> f
            | FlagAndValue (f,s,v) -> $"{f}{s}{v}"
            | ValueOnly v -> v
                
        // Parse the given string as an Arg
        let parse (input: string) : Arg =
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
            let assign_match = assign_regex.Match input 
            if assign_match.Success then
                flag <- assign_match.Groups[1].Value
                sep <- assign_match.Groups[2].Value
                value <- assign_match.Groups[3].Value
            else
                value <- input
            
            let value_match =
                value_regex.Match <| value.Trim()
                
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
            
    /// Command line arguments
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
                
        static member create([<ParamArray>] args: obj[]) =
            args
            |> Seq.map string
            |> Args.parseMany
            
        member this.append(arg: Arg) = 
            match this with
            | Args xs -> Args (xs @ [arg])
        
        member this.append(Args args: Args) = 
            match this with
            | Args xs -> Args (xs @ args)
            
        member this.append([<ParamArray>] args: obj[]) =
            let args' =
                args
                |> Seq.map string
                |> Args.parseMany
            
            (this.append: Args -> Args)(args')
            
        static member (+) (a: Args, b: Args) =
            match a,b with
            | Args a', Args b' -> Args (a' @ b')
            
        static member (+) (a: Args, b: string) =
            a + (Args.parse b)
            
        static member (+) (a: Args, b: Arg) =
            a + (Args [b])
    
        
    /// Command line arguments        
    module Args =
        
        /// Empty argument list
        let empty =
            Args []
        
        /// Apply the given function to the arguments
        let map f args =
            match args with
            | Args args -> Args (f args)
        
        /// Add an argument
        let append arg args =
            map (fun args -> args @ [arg]) args
            
        /// Get the list of args            
        let toList args =
            match args with
            | Args args -> args
            
        /// Return the args as a string
        let toString args =
            args
            |> Seq.map string
            |> String.concat " "
            
        /// Parse arguments from the given string            
        let parse (input: string) =
            let tokens =
                input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            let args =
                tokens
                |> Seq.map Arg.parse
                |> Seq.toList
                |> Args
            args
            
        /// Parse args from multiple strings
        let parseMany (strings: string seq) =
            strings
            |> Seq.map parse
            |> Seq.collect toList
            |> Seq.toList
            |> Args
            
        let ofObsj (objs: obj[]) =
            objs
            |> Seq.map string
            |> parseMany
            
        let ofSeq (args: Arg seq) =
            args
            |> Seq.toList
            |> Args
            
        let ofList (args: Arg list) =
            Args args

    /// A command to be executed
    type Command =
        { Exe   : string
          Args  : Args }
            
        member this.Statement =
            Command.statement this
            
        /// Run this command            
        member this.Run() =
            Command.run this
            
        /// Run this command and wait for it to complete            
        member this.RunSync() =
            Command.runSync this

        /// Run this command and yield back messages are they are produced                        
        member this.Stream() =
            Command.stream this

        /// Create a command from the given args            
        static member Create([<ParamArray>] args: obj[]) =
            args
            |> Seq.map string
            |> Args.parseMany
            |> Command.ofArgs

            
    module Command =

        let stdout (result: CommandResult) =
            result.StdOut
        
        let stderr (result: CommandResult) =
            result.StdErr
                
        let map f (result: CommandResult) =
            match result.ExitCode with
            | 0 ->
                Result.Ok (f result)
            | _ ->
                Result.Error result.StdErr
            
        let empty =
            { Exe = ""; Args = Args.empty; }
            
        let create (exe: string) (args: Args) =
            { Exe = exe; Args = args }
            
        let ofArgs (Args.Args args: Args) =
            match args with
            | [] ->
                failwith $"No args given"
            | exe :: args ->
                create exe.Value (Args.ofList args)
                
        /// Replace the arguments with the given ones
        let withArgs (args: Args) (cmd: Command) =
            { cmd with Args = args }
        
        /// Return the full command line statement of
        /// a command
        let statement (cmd: Command) =
            let statement = $"{cmd.Exe} {cmd.Args}"  
            statement

        /// Create a ProcessStartInfo for the given command        
        let toStartInfo (cmd: Command) =
            let info = ProcessStartInfo()
            info.FileName <- cmd.Exe
            info.RedirectStandardOutput <- true
            info.RedirectStandardError <- true
            info.UseShellExecute <- false
            info.CreateNoWindow <- true
            for arg in cmd.Args do
                info.ArgumentList.Add (string arg)
            info
        
        /// Create a new Process for the given command
        let toProcess (cmd: Command) =
            let proc = new Process()
            proc.EnableRaisingEvents <- true
            proc.StartInfo <- toStartInfo cmd
            proc
            
        /// Start the given command and stream the output back asynchronously
        let stream (cmd: Command) : IAsyncEnumerable<CommandMessage> =
            
            let proc =
                toProcess cmd
                
            let channel =
                Channel.CreateUnbounded<CommandMessage>()
                
            let mutable startMessage =
                Unchecked.defaultof<CommandMessage>
                
            let handleData status (args: DataReceivedEventArgs) =
                match args.Data with
                | null ->
                    ()
                | text ->
                    let message =
                        { startMessage with
                            Message = text
                            Status = status
                            TimeStamp = DateTimeOffset.Now
                            Elapsed = DateTimeOffset.Now - startMessage.StartTime }

                    channel.Writer.TryWrite message
                    |> ignore
                    
            let handleExit _ =
                let message =
                    { startMessage with
                        Status = if proc.ExitCode > 0 then CommandStatus.Success else CommandStatus.Failure
                        TimeStamp = DateTimeOffset.Now
                        Elapsed = DateTimeOffset.Now - startMessage.StartTime }
                    
                proc.Dispose()
                channel.Writer.TryWrite message
                channel.Writer.Complete()

            proc.OutputDataReceived.Add (handleData CommandStatus.StdOut)
            proc.ErrorDataReceived.Add (handleData CommandStatus.StdErr)
            proc.Exited.Add handleExit
            proc.Start()
            
            startMessage <-
                { ProcessId = proc.Id
                ; Message = ""
                ; Status = CommandStatus.Started
                ; StartTime = DateTimeOffset.Now 
                ; TimeStamp = DateTimeOffset.Now
                ; Elapsed = TimeSpan.Zero }
            
            channel.Writer.TryWrite startMessage
            proc.BeginOutputReadLine()
            proc.BeginErrorReadLine()
            channel.Reader.ReadAllAsync()        
                    
        /// Execute the given Command 
        let run (cmd: Command) =
            let proc = toProcess cmd
            let statement = Command.statement cmd
            let stdout = StringBuilder()
            let stderr = StringBuilder()
            let complete = TaskCompletionSource<bool>()
            
            let handleData (builder: StringBuilder) (args: DataReceivedEventArgs) =
                match args.Data with
                | null ->
                    ()
                | msg ->
                    ignore (builder.Append msg)
            
            proc.OutputDataReceived.Add (handleData stdout)
            proc.ErrorDataReceived.Add (handleData stderr)
            proc.Exited.Add (fun _ -> complete.SetResult true)
                    
            task {
                let start_time = DateTimeOffset.Now
                
                proc.Start()
                proc.BeginOutputReadLine()
                proc.BeginErrorReadLine()
                        
                let! _ = complete.Task
                let end_time = DateTimeOffset.Now
                
                let result =
                    { Command = proc.StartInfo.FileName
                    ; Args = proc.StartInfo.ArgumentList |> Seq.toList
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
            
        /// Execute the given Command and wait for the result 
        let runSync (cmd: Command) =
            use proc = toProcess cmd
            let statement = Command.statement cmd
            let stdout = StringBuilder()
            let stderr = StringBuilder()
                        
            let handleData (builder: StringBuilder) (args: DataReceivedEventArgs) =
                match args.Data with
                | null ->
                    ()
                | msg ->
                    ignore (builder.Append msg)
            
            proc.OutputDataReceived.Add (handleData stdout)
            proc.ErrorDataReceived.Add (handleData stderr)
        
            let start_time = DateTimeOffset.Now
            
            proc.Start()
            proc.BeginOutputReadLine()
            proc.BeginErrorReadLine()
            proc.WaitForExit()
            
            let end_time = DateTimeOffset.Now
            
            let result =
                { Command = proc.StartInfo.FileName
                ; Args = proc.StartInfo.ArgumentList |> Seq.toList
                ; Statement = statement 
                ; StartTime = start_time
                ; EndTime = end_time
                ; Duration = end_time - start_time
                ; ExitCode = proc.ExitCode
                ; IsError = proc.ExitCode > 0 
                ; StdOut = string stdout
                ; StdErr = string stderr }
            
            result