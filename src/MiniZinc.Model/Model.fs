namespace MiniZinc

open System.Runtime.InteropServices
open System
open System.IO
    
[<AutoOpen>]
module rec Model =
    
    // A MiniZinc model
    type Model =
        { Name         : string
        ; Includes     : IncludeItem list
        ; Enums        : EnumItem list
        ; Aliases      : AliasItem list
        ; Variables    : DeclareItem list
        ; Assigns      : AssignItem list
        ; Constraints  : ConstraintItem list
        ; Predicates   : PredicateItem list
        ; Functions    : FunctionItem list
        ; Inputs       : DeclareItem list
        ; Decisions    : DeclareItem list
        ; Outputs      : OutputItem list 
        ; Solve        : SolveItem }
                
        /// <summary>
        /// Parse a Model from the given file
        /// </summary>
        static member parseFile (file: string) =
            Model.parseFile (FileInfo file)

        /// <summary>
        /// Parse a Model from the given string
        /// </summary>
        /// <param name="allowEmpty">
        /// If true, an empty model will be returned
        /// in the case of an empty/null string.
        ///
        /// Defaults is false.
        /// </param>
        static member parseString ([<Optional; DefaultParameterValue(false)>] allowEmpty: bool) =
            fun input ->
                if String.IsNullOrWhiteSpace input && allowEmpty then
                    Result.Ok Model.empty
                else
                    Model.parseString input
                
                
    module Model =
        
        let empty =
            { Name = ""
            ; Includes = []
            ; Enums = []
            ; Aliases = []
            ; Variables = []
            ; Assigns = []
            ; Constraints = []
            ; Functions = []
            ; Predicates = []
            ; Solve = SolveItem.Satisfy 
            ; Inputs = []
            ; Decisions = []
            ; Outputs = [] }
        
        let parseFile file : Result<Model, ParseError> =
            
            let string =
                File.ReadAllText file.FullName
            
            let model =
                parseString string
                
            model
            
        let parseString string : Result<Model, ParseError> =
            
            let input, comments =
                Parse.sanitize string
            
            let model =
                input
                |> Parse.string Parsers.ast
                |> Result.map fromAst
                
            model                

        // Create a Model from the given AST            
        let fromAst ast : Model =
            addAst empty ast
            
        // Incorporate the given AST into the model
        let addAst (model: Model) (ast: Ast) : Model =
            
            let rec loop model ast =
                match ast with
                | [] ->
                    model
                | item :: rest ->
                    match item with
                    | Include x ->
                        loop {model with Includes = x :: model.Includes} rest
                    | Enum x ->
                        loop {model with Enums = x :: model.Enums} rest
                    | Alias x ->
                        loop {model with Aliases =  x :: model.Aliases} rest
                    | Constraint x ->
                        loop {model with Constraints =  x :: model.Constraints} rest
                    | Assign x ->
                        loop {model with Assigns =  x :: model.Assigns} rest
                    | Declare x ->
                        loop {model with Variables =   x :: model.Variables} rest
                    | Solve x ->
                        loop {model with Solve = x} rest
                    | Predicate x ->
                        loop {model with Predicates =  x :: model.Predicates} rest
                    | Function x ->
                        loop {model with Functions =   x :: model.Functions} rest
                    | Output x ->
                        loop {model with Outputs = x :: model.Outputs} rest
                    | Annotation _ ->
                        loop model rest
                    | Comment _ ->
                        loop model rest
                    | Test _ ->
                        loop model rest                        

            let result =
                ast
                |> loop model
                |> reconcile 

            result
    
        
    /// <summary>
    /// Reconcile (validate? post process?) the model
    /// </summary>
    /// <remarks>
    /// Performs all necessary transformations on the model
    /// to ensure all our desired information is computed.
    /// </remarks>        
    let reconcile (model: Model) =
        
        let vars =
            resolveVariables model
            
        let decisions =
            vars
            |> List.choose (fun var -> 
                match var.Value with
                | None when var.Inst = Inst.Var ->
                    Some var
                | _ ->
                    None
            )
             
        let inputs =
            vars
            |> List.choose (fun var -> 
                match var.Value with
                | None when var.Inst = Inst.Par ->
                    Some var
                | _ ->
                    None
            )            
            
        let result =
            { model with
                Variables = vars
                Decisions = decisions
                Inputs = inputs }
            
        result            
                                

    /// <summary>
    /// Infer variables for the model
    /// </summary>
    /// <remarks>
    /// Combine the information from variable
    /// Declarations and Assignments to deteremine
    /// what variables have been assigned or not.
    /// </remarks>
    let resolveVariables (model: Model) =
        
        let vars =
            model.Variables
            |> Map.withKey (fun v -> v.Name)
            
        let assign vars (name, value) =
            match Map.tryFind name vars with
            | None ->
                vars
            | Some var ->
                Map.add name {var with Value = Some value } vars
        
        let vars =
            model.Assigns
            |> List.fold assign vars
            |> Map.values
            |> Seq.toList
   
        vars
        
        