namespace MiniZinc.TestSpec.Model;

public sealed record TestCaseSolution(IReadOnlyDictionary<string, TestCaseSolutionValue> Variables)
{
    public bool HasOutputItem => Variables.ContainsKey("_output_item");
    public bool HasChecker => Variables.ContainsKey("_checker");
}
