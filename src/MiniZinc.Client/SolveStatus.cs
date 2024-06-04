namespace MiniZinc.Client;

public enum SolveStatus : short
{
    Started,
    Satisfied,
    SubOptimal,
    Optimal,
    AllSolutions,
    Unsatisfiable,
    Unbounded,
    UnsatOrUnbounded,
    Timeout,
    Error
}
