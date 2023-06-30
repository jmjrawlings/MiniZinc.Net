namespace MiniZinc

open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Nodes
open System.Text.Json.Serialization
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
    
    type Convergence<'t> =
        { Objective : 't
        ; Bound: 't
        ; AbsoluteGap : 't
        ; RelativeGap: float
        ; AbsoluteDelta: 't
        ; RelativeDelta: float }
        
    type SolutionStatus =
        | Started 
        | Satisfied
        | SubOptimal of Objective
        | Optimal of Objective
        | Unsatisfiable
        | Unbounded
        | AllSolutions
        | Timeout of TimeSpan
        | Error of string
        
    type Solution =
        { Command    : string
        ; ProcessId  : int
        ; TotalTime  : TimePeriod
        ; IterationTime : TimePeriod
        ; Iteration  : int
        ; Variables  : Map<string, Expr>
        ; Statistics : Map<string, JsonValue>
        ; Status     : SolutionStatus }
        
    type Objective =
        | Int of int
        | Float of float
        
    module MiniZincClient =
        
        open Command
        
        /// Solve the given model                            
        let solve (model: Model) (client: MiniZincClient) : IAsyncEnumerable<Solution> =

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
                
            let startTime =
                DateTimeOffset.Now
                
            let mutable solution = Unchecked.defaultof<Solution>
            
            taskSeq {
                for msg in command.Stream() do
                    match msg.Status with
                    
                    // MiniZinc has started
                    | CommandStatus.Started ->
                        solution <-
                            { Command = msg.Command
                            ; ProcessId = msg.ProcessId
                            ; Iteration = 0
                            ; TotalTime = TimePeriod.At startTime
                            ; IterationTime =  TimePeriod.At startTime
                            ; Variables = Map.empty
                            ; Statistics = Map.empty 
                            ; Status = SolutionStatus.Started }
                        yield solution
                        
                    // Standard Output Received
                    | CommandStatus.StdOut ->
                        let json = JsonObject.Parse(msg.Message).AsObject()
                        match json["type"].GetValue<string>() with
                        | "solution" ->

                            let dzn =
                                (json["output"]["dzn"]).GetValue<string>()

                            let vars, obj =
                                match (parseDataString dzn) with
                                | Result.Error err ->
                                    failwith $"{err}"
                                | Result.Ok map when map.ContainsKey "_objective" ->
                                    let obj = map["_objective"]
                                    let vars = Map.remove "_objective" map
                                    vars, (Some obj)
                                | Result.Ok map ->
                                    map, None
                                    
                            let status =
                                match obj with
                                | Some (Expr.Int i) ->
                                    SolutionStatus.SubOptimal (Objective.Int i)
                                | Some (Expr.Float f) ->
                                    SolutionStatus.SubOptimal (Objective.Float f) 
                                | _ ->
                                    SolutionStatus.Satisfied

                            let sol =
                                { solution with
                                    Status = status
                                    Variables = vars
                                    TotalTime = TimePeriod.Create(startTime, msg.TimeStamp) 
                                    IterationTime = TimePeriod.Since(solution.IterationTime)
                                    }
                                
                            solution <- sol                                
                            yield sol
                            
                        | "statistics" ->
                            let statsJson = json["statistics"]
                            let stats = JsonSerializer.Deserialize<Map<string, JsonValue>>(statsJson)
                            let allStats = Map.merge solution.Statistics stats
                            solution <- { solution with Statistics = allStats }
                            ()
                        | other ->
                            ()
                    | CommandStatus.StdErr ->
                        ()
                    | CommandStatus.Success ->
                        ()
                    | CommandStatus.Failure ->
                        ()
                    | _ ->
                        ()
                        
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
            
                        