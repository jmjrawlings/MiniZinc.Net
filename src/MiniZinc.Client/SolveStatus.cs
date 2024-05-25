namespace MiniZinc.Client;

public enum SolveStatus : short
{
    Pending,
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
