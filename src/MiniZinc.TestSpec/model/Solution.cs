namespace MiniZinc.TestSpec.Model;

public sealed record Solution(IReadOnlyDictionary<string, SolutionValue> Variables)
{
    public bool HasOutputItem => Variables.ContainsKey("_output_item");
    public bool HasChecker => Variables.ContainsKey("_checker");
}
