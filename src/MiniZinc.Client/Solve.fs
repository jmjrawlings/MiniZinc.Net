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
        | AllSolutions = 3
        | Unsatisfiable = 4
        | Unbounded = 5
        | UnsatOrUnbounded = 6
        | Timeout = 7
        | Error = 8
        
    module StatusType =
        let parse input =
            match input with
            | "ALL_SOLUTIONS" ->
                StatusType.AllSolutions
            | "OPTIMAL_SOLUTION" ->
                StatusType.Optimal
            | "UNSATISFIABLE" ->
                StatusType.Unsatisfiable
            | "UNBOUNDED" ->
                StatusType.Unbounded
            | "UNSAT_OR_UNBOUNDED" ->
                StatusType.UnsatOrUnbounded
            | _ ->
                StatusType.Error
                
        let isSuccess status =
            status <= StatusType.AllSolutions
        
        let isError status =
            status >= StatusType.Unsatisfiable
    
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
        { Command     : string
        ; ProcessId   : int
        ; TotalTime   : TimePeriod
        ; IterationTime : TimePeriod
        ; Iteration   : int
        ; Decisions   : Map<string, TypeInst>
        ; Outputs     : Map<string, Expr>
        ; Statistics  : Map<string, JsonValue>
        ; Status      : SolutionStatus }
        
    type Objective =
        | Int of int
        | Float of float
        
    module MiniZincClient =
        
        /// Solve the given model                            
        let solve (compiled: CompiledModel) (client: MiniZincClient) : IAsyncEnumerable<Solution> =
            
            let model = compiled.Model
            
            let command =
                client.Command(
                    "--json-stream",
                    "--output-objective",
                    "--statistics",
                    compiled.ModelArg
                )
                
            let startTime =
                DateTimeOffset.Now
                
            let mutable solution =
                Unchecked.defaultof<Solution>
            
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
                            ; Decisions = model.NameSpace.Outputs
                            ; Outputs = Map.empty 
                            ; Statistics = Map.empty 
                            ; Status = SolutionStatus.Started }
                        yield solution
                        
                    // Standard Output Received
                    | CommandStatus.StdOut ->
                        
                        let json =
                            JsonSerializer.Deserialize<JsonObject>(msg.Message)
                            
                        let outputType =
                            json["type"].GetValue<string>()
                        
                        match outputType with
                        
                        | "solution" ->

                            let dataString =
                                (json["output"]["dzn"]).GetValue<string>()

                            let assignment, obj =
                                match (parseDataString dataString) with
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
                                    Outputs = assignment
                                    TotalTime = TimePeriod.Create(startTime, msg.TimeStamp) 
                                    IterationTime = TimePeriod.Since(solution.IterationTime) }
                                
                            solution <- sol                                
                            yield sol
                            
                        | "statistics" ->
                                
                            let stats =
                                JsonSerializer.Deserialize<Map<string, JsonValue>>(json["statistics"])
                                
                            let allStats =
                                Map.merge solution.Statistics stats
                                
                            solution <- { solution with Statistics = allStats }
                            ()
                            
                        | "status" ->
                            
                            let status = StatusType.parse
                            ()
                            
                        | other ->
                            notImpl other
                            
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
        let solveSync (model: CompiledModel) (client: MiniZincClient) =
            
            let solution =
                client
                |> MiniZincClient.solve model
                |> TaskSeq.last
                |> fun x -> x.Result
                
            solution


    type MiniZincClient with
    
        /// Solve the given model string      
        member this.Solve(model: CompiledModel) =
            MiniZincClient.solve model this

        /// Solve the given model string      
        member this.Solve(model: string) =
            let model = Model.ParseString(model).Model
            let compiled = this.Compile(model)
            this.Solve compiled

        /// Solve the given model
        member this.Solve(model: Model) =
            let compiled = this.Compile(model)
            this.Solve(compiled)
           
        /// Solve the given model and wait for the last solution            
        member this.SolveSync(model: Model) =
            
            let solution =
                this.Solve(model)
                |> TaskSeq.last
                |> fun x -> x.Result
            
            solution
            
        /// Solve the given model and wait for the last solution            
        member this.SolveSync(model: string) =

            let solution =
                this.Solve(model)
                |> TaskSeq.last
                |> fun x -> x.Result
            
            solution