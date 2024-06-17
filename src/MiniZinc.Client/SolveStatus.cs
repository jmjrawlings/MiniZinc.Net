namespace MiniZinc.Client;

public enum SolveStatus : short
{
    Satisfied = 1,
    SubOptimal,
    Optimal,
    AllSolutions,
    Unsatisfiable,
    Unbounded,
    UnsatOrUnbounded,
    Timeout,
    Error,
    SyntaxError,
    TypeError,
    AssertionError,
    EvaluationError
}
