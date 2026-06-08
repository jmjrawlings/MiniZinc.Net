namespace MiniZinc.TestSpec.Model;

public abstract record Expected;

public sealed record ExpectedSolution(TestCaseSolution TestCaseSolution) : Expected;

public sealed record ExpectedSolutionSet(IReadOnlyList<TestCaseSolution> Solutions) : Expected;

public sealed record ExpectedAllSolutions(IReadOnlyList<TestCaseSolution> Solutions) : Expected;

public sealed record ExpectedUnsatisfiable : Expected;

public sealed record ExpectedError(ErrorKind Kind, string? Message, string? Regex) : Expected;

public sealed record ExpectedFlatZinc(string RelativePath) : Expected;

public sealed record ExpectedOutputModel(string RelativePath) : Expected;

public sealed record ExpectedCheckAgainst(IReadOnlyList<string> Solvers) : Expected;
