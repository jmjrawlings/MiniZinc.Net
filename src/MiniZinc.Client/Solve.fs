namespace MiniZinc

open System.Collections.Generic
open FSharp.Control

[<AutoOpen>]
module Solve =
    open Command
        
    module MiniZincClient =
        
        /// Solve the given model                            
        let solve (model: Model) (mz: MiniZincClient) : IAsyncEnumerable<OutputMessage> =
            let model_file = MiniZincClient.write_model_to_tempfile model
            let model_arg = MiniZincClient.model_arg model_file.FullName
            let command = mz.Command("--json-stream", model_arg)
                
            command
            |> Command.stream
            |> TaskSeq.choose (function
                | CommandMessage.Output msg ->
                    Some msg
                | _ ->
                    None)

        /// Solve the given model and wait for the best solution only
        let solveSync (model: Model) (client: MiniZincClient) =
            
            let task =
                client
                |> solve model
                |> TaskSeq.last
                
            let result =
                task.Result
                
            result


    type MiniZincClient with

        /// Solve the given model      
        member this.Solve(model: Model) =
            MiniZincClient.solve model this
            
        /// Solve the given model and wait for the last solution            
        member this.SolveSync(model: Model) =
            MiniZincClient.solveSync model this