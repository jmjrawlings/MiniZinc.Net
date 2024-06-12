namespace LibMiniZinc.Tests;

public enum TestType
{
    Compile,
    Satisfy,
    AnySolution,
    AllSolutions,
    Optimise,
    OutputModel,
    Unsatisfiable,
    AssertionError,
    EvaluationError,
    MiniZincError,
    TypeError,
    SyntaxError,
    Error
}
