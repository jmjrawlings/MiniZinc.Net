namespace MiniZinc

open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Nodes
open FSharp.Control
open MiniZinc.Command
open MiniZinc.Parser
open Microsoft.Extensions.Logging

[<AutoOpen>]
module rec Solve =
        
    type SolveOptions =
        { Solver : string
        ; Timeout : TimeSpan }
        
        static member Default =
            SolveOptions.create "gecode"
            
        static member Create(
            solver: string,
            timeout: TimeSpan) =
            { Solver = solver
            ; Timeout = timeout }
        
    module SolveOptions =
        
        let create solver =
            { Solver = solver
            ; Timeout = TimeSpan.Zero }
    
        
    type StatusType =
        | Started = -1
        | Satisfied = 0
        | SubOptimal = 1
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
            | "SATISFIED" ->
                StatusType.Satisfied
            | "OPTIMAL" ->
                StatusType.Optimal
            | other ->                
                failwith $"Unexpected status type \"{other}\""
                
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
        | SubOptimal of Expr
        | Optimal of Expr
        | Unsatisfiable
        | Unbounded
        | AllSolutions
        | Timeout of TimeSpan
        | Error of JsonObject
        
        member this.Type =
            match this with
            | Started -> StatusType. Started
            | Satisfied -> StatusType.Satisfied
            | SubOptimal _ -> StatusType.SubOptimal
            | Optimal _ -> StatusType.Optimal
            | Unsatisfiable -> StatusType.Unsatisfiable
            | Unbounded -> StatusType.Unbounded
            | AllSolutions -> StatusType.AllSolutions
            | Timeout _ -> StatusType.Timeout
            | Error _ -> StatusType.Error
        
    type Solution =
        { Command       : string
        ; ProcessId     : int
        ; TotalTime     : TimePeriod
        ; IterationTime : TimePeriod
        ; Iteration     : int
        ; Variables     : Map<string, TypeInst>
        ; Outputs       : Map<string, Expr>
        ; Statistics    : Map<string, JsonValue>
        ; Status        : SolutionStatus
        ; StatusType    : StatusType
        ; Warnings      : JsonObject list }
        
        member this.IsSuccess =
            StatusType.isSuccess this.StatusType
            
        member this.IsError =
            StatusType.isError this.StatusType
            
        member this.Objective =
            match this.Status with
            | SolutionStatus.SubOptimal obj
            | SolutionStatus.Optimal obj ->
                Some obj
            | _ ->
                None
            
        override this.ToString() =
            $"<Solution \"{this.StatusType}\""
        
    module MiniZincClient =
        
        /// Solve the given model                            
        let solve (compiled: CompiledModel) (options: SolveOptions) (client: MiniZincClient) : IAsyncEnumerable<Solution> =
            
            let model =
                compiled.Model
                
            client.Logger.LogInformation("Solving model {Name}", model.Name)
            
            let command =
                client.Command(
                    "--json-stream",
                    "--output-objective",
                    $"--solver {options.Solver}",
                    //"--statistics",
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
                            ; Variables = model.NameSpace.Variables
                            ; Outputs = Map.empty 
                            ; Statistics = Map.empty 
                            ; Status = SolutionStatus.Started
                            ; StatusType = StatusType.Started 
                            ; Warnings = [] }
                            
                        yield solution
                        
                    // Standard Output Received
                    | CommandStatus.StdOut ->
                        
                        let message =
                            JsonSerializer.Deserialize<JsonObject>(msg.Message)
                            
                        let messageType =
                            message["type"].GetValue<string>()
                        
                        match messageType with
                                                
                        // A solution has been found                        
                        | "solution" ->
                            
                            let dataString =
                                (message["output"]["dzn"]).GetValue<string>()
                                                        
                            let data =
                                match parseDataString ParseOptions.Default dataString with
                                | Result.Ok items ->
                                    items
                                    |> Seq.map (fun struct(id,expr) -> (id,expr))
                                    |> Map.ofSeq
                                    |> Result.Ok
                                | Result.Error err ->
                                    Result.Error err

                            let outputs, status =
                                match data with
                                | Result.Error err ->
                                    let exn = MiniZincException(
                                        $"Could not parse the solution string",
                                        err.Message)
                                    raise exn
                                | Result.Ok vars when vars.ContainsKey "_objective" ->
                                    let obj = vars["_objective"]
                                    let vars = Map.remove "_objective" vars
                                    vars, (SolutionStatus.SubOptimal obj)
                                | Result.Ok vars ->
                                    vars, SolutionStatus.Satisfied

                            solution <-
                                { solution with
                                    Status = status
                                    StatusType = StatusType.Started
                                    Outputs = outputs
                                    Iteration = solution.Iteration + 1 
                                    TotalTime = TimePeriod.Create(startTime, msg.TimeStamp) 
                                    IterationTime = TimePeriod.Since(solution.IterationTime) }
                                                            
                            yield solution
                            
                        | "statistics" ->
                                
                            let statistics =
                                message["statistics"]
                                |> JsonSerializer.Deserialize<Map<string, JsonValue>>
                                
                            solution <-
                                { solution with
                                    Statistics =
                                        Map.merge solution.Statistics statistics }
                                
                            ()
                            
                        | "status" ->
                            
                            let statusType =
                                message["status"].GetValue<string>()
                                |> StatusType.parse
                            
                            let status =
                                match statusType with
                                
                                | StatusType.Satisfied ->
                                    SolutionStatus.Satisfied
                                    
                                | StatusType.Optimal->
                                    match solution.Status with
                                    | SolutionStatus.SubOptimal obj ->
                                        SolutionStatus.Optimal obj
                                    | bad ->
                                        failwithf $"Unexpected status {bad}"
                                        
                                | StatusType.AllSolutions ->
                                    SolutionStatus.AllSolutions
                                    
                                | StatusType.Unsatisfiable ->
                                    SolutionStatus.Unsatisfiable
                                    
                                | StatusType.Unbounded ->
                                    SolutionStatus.Unbounded
                                    
                                | x ->
                                    failwithf $"Unexpected status {x}"

                            let iteration =
                                match StatusType.isSuccess statusType with
                                | true -> solution.Iteration + 1
                                | false -> solution.Iteration
                        
                            solution <-
                                { solution with
                                    Status = status
                                    StatusType = status.Type
                                    Iteration = iteration
                                    TotalTime = TimePeriod.Create(startTime, msg.TimeStamp) 
                                    IterationTime = TimePeriod.Since(solution.IterationTime) }
                            
                        | "error" ->

                            let status =
                                SolutionStatus.Error message
                                
                            solution <-
                                { solution with
                                    Status = status
                                    StatusType = status.Type 
                                    TotalTime = TimePeriod.Create(startTime, msg.TimeStamp) 
                                    IterationTime = TimePeriod.Since(solution.IterationTime) }
                        
                        | "warning" ->
                            
                            let warning =
                                message
                            
                            solution <-
                                { solution with
                                    Warnings = message :: solution.Warnings}
                            
                        | other ->
                            notImpl other
                            
                    | CommandStatus.StdErr ->
                        failwith msg.Message
                        
                    | CommandStatus.Success ->
                        yield solution
                        
                    | CommandStatus.Failure ->
                        yield solution
                        
                    | _ ->
                        failwith msg.Message
            }
            
        /// Solve the given model and wait for the best solution only
        let solveSync (model: CompiledModel) (options: SolveOptions) (client: MiniZincClient) =
            
            let solution =
                client
                |> MiniZincClient.solve model options
                |> TaskSeq.last
                |> fun x -> x.Result
                
            solution


    type MiniZincClient with
        
        /// Solve the given model string      
        member this.Solve(model: CompiledModel, options: SolveOptions) =
            MiniZincClient.solve model options this

        /// Solve the given model string      
        member this.Solve(model: string, options: SolveOptions) =
            let model = Model.ParseString(model).Get()
            let compiled = this.Compile(model)
            this.Solve(compiled, options)

        /// Solve the given model string      
        member this.Solve(model: string) =
            this.Solve(model, SolveOptions.Default)
        
        /// Solve the given model
        member this.Solve(model: Model, options: SolveOptions) =
            let compiled = this.Compile(model)
            this.Solve(compiled, options)
            
        member this.Solve(model: Model) =
            this.Solve(model, SolveOptions.Default)
           
        /// Solve the given model and wait for the last solution
        member this.SolveSync(model: Model, options: SolveOptions) =
            
            let solution =
                this.Solve(model, options)
                |> TaskSeq.last
                |> fun x -> x.Result
            
            solution
            
        member this.SolveSync(model: Model) =
            this.SolveSync(model, SolveOptions.Default)
         
        member this.SolveSync(mzn: string, options) =
            let model = Model.ParseString(mzn).Get()
            this.SolveSync(model, options)
            
        member this.SolveSync(mzn: string) =
            this.SolveSync(mzn, SolveOptions.Default)            