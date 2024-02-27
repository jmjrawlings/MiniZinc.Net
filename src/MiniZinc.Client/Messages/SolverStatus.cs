namespace MiniZinc.Client.Messages;

public enum SolverStatus
{
    ALL_SOLUTIONS,
    OPTIMAL_SOLUTION,
    UNSATISFIABLE,
    UNBOUNDED,
    UNSAT_OR_UNBOUNDED,
    UNKNOWN,
    ERROR,
}
