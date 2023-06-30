namespace MiniZinc

open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Nodes
open FSharp.Control
open MiniZinc.Command

[<AutoOpen>]
module rec Solve =
        
    type StatusType =
        | Satisfied = 0
        | Suboptimal = 1
        | Optimal = 2
        | Timeout = 3
        | Unsatisfiable = 4
        | Error = 5
        | Unbounded = 6 
        | AllSolutions = 7

    type SolutionType =
        | Satisfied = 0
        | Suboptimal = 1
        | Optimal = 2
    
    type ObjectiveInfo =
        | Satisfied
        | Int of int
        | Float of float
        | ConvergingInt of Convergence<int>
        | ConvergingFloat of Convergence<float>
        
    type Convergence<'t> =
        { Objective : 't
        ; Bound: 't
        ; AbsoluteGap : 't
        ; RelativeGap: float
        ; AbsoluteDelta: 't
        ; RelativeDelta: float }
        
    type SolveStatus =
        | Satisfied
        | Suboptimal of ObjectiveInfo
        | Optimal of ObjectiveInfo
        | Unsatisfiable
        | Unbounded
        | AllSolutions
        | Timeout of TimeSpan
        | Error of string
        
    type SolveSummary =
        { StartTime : DateTime
        ; EndTime   : DateTime
        ; Duration  : TimeSpan
        ; Status    : SolveStatus }
       
    type Solution =
        { Variables : Map<string, Expr>
        ; Type : SolutionType
        ; Objective : ObjectiveInfo }
        
    module MiniZincClient =
        
        open Command
        
        /// Solve the given model                            
        let solve (model: Model) (client: MiniZincClient) : IAsyncEnumerable<CommandMessage> =

            let model_mzn =
                MiniZincClient.compile model
                
            let model_arg =
                MiniZincClient.model_arg model_mzn.FullName
                
            let command =
                client.Command(
                    "--json-stream",
                    "--output-objective",
                    "--statistics",
                    model_arg
                )
            
            taskSeq {
                for msg in command.Stream() do
                    match msg.Status with
                    | CommandStatus.Started ->
                        yield msg
                    | CommandStatus.StdOut ->
                        let json = JsonObject.Parse(msg.Message).AsObject()
                        match json["type"].GetValue<string>() with
                        | "solution" ->
                            let dzn = (json["output"]["dzn"]).GetValue<string>()
                            let vars = (parseDataString dzn).Get()
                            yield msg
                        | "statistics" ->
                            yield msg
                        | other ->
                            yield msg
                    | CommandStatus.StdErr ->
                        yield msg
                    | CommandStatus.Success ->
                        yield msg
                    | CommandStatus.Failure ->
                        yield msg
                    | _ ->
                        yield msg
                        
            }
            
        /// Solve the given model and wait for the best solution only
        let solveSync (model: Model) (client: MiniZincClient) =
            
            let model_file = MiniZincClient.compile model
            let model_arg = MiniZincClient.model_arg model_file.FullName
            let command = client.Command(
                "--json-stream",
                "--output-objective",
                "--statistics",
                model_arg
                )
            let result = command.RunSync()
            result


    type MiniZincClient with
    
        /// Solve the given model string      
        member this.Solve(model: string) =
            let model = Model.ParseString(model).Model
            MiniZincClient.solve model this

        /// Solve the given model      
        member this.Solve(model: Model) =
            MiniZincClient.solve model this
           
        /// Solve the given model and wait for the last solution            
        member this.SolveSync(model: Model) =
            MiniZincClient.solveSync model this
            
        /// Solve the given model and wait for the last solution            
        member this.SolveSync(model: string) =
            let model = Model.ParseString(model).Model
            MiniZincClient.solveSync model this