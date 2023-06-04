namespace MiniZinc
open MiniZinc

module Encode =
     
    let includeItem (x: IncludeItem) =
        string x
    
    let enumItem (x: EnumItem) =
        // { Name : Id
        // ; Annotations : Annotations
        // ; Cases : EnumCase list }
        ""
        
    let enumCase (x: EnumCase) =
        ""
    
    let synonym (x: SynonymItem) =
        ""
        
    let constraintItem (x: ConstraintItem) =
        ""
        
    let assignItem (x: AssignItem) =
        ""
        
    let declareItem (x: DeclareItem) =
        ""
        
    let solveMethod (x: SolveMethod) =
        ""
        
    let predicateItem (x: PredicateItem) =
        ""
        
    let functionItem (x: FunctionItem) =
        ""
        
    let testItem (x: TestItem) =
        ""
        
    let outputItem (x: OutputItem) =
        ""
        
    let annotationItem (x: AnnotationItem) =
        ""
        
    let comment (x: string) =
        x
        
        
    