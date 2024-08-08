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
    Error,
    AssertionError,
    EvaluationError,
    MiniZincError,
    TypeError,
    SyntaxError
}
