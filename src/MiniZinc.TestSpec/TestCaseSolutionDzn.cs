namespace MiniZinc.TestSpec;

using System.Globalization;
using System.Text;
using MiniZinc.TestSpec.Model;

/// <summary>
/// Renders a parsed <see cref="TestCaseSolution"/> back into a DZN fragment
/// (<c>name=value;…</c>) so it can be round-tripped through the MiniZinc parser
/// and compared against an actual solver result by the integration tests.
///
/// Returns <c>null</c> when the solution has no comparable assignments — e.g.
/// an <c>_output_item</c>/<c>_checker</c> solution, which describes raw output
/// text rather than variable bindings.
/// </summary>
public static class TestCaseSolutionDzn
{
    public static string? Render(TestCaseSolution solution)
    {
        if (solution.HasOutputItem || solution.HasChecker)
            return null;

        if (solution.Variables.Count == 0)
            return null;

        var sb = new StringBuilder();
        foreach (var (name, value) in solution.Variables)
        {
            sb.Append(name);
            sb.Append('=');
            RenderValue(value, sb);
            sb.Append(';');
        }
        return sb.ToString();
    }

    private static void RenderValue(TestCaseSolutionValue value, StringBuilder sb)
    {
        switch (value)
        {
            case IntVal i:
                sb.Append(i.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case FloatVal f:
                sb.Append(f.Value.ToString(CultureInfo.InvariantCulture));
                break;
            case BoolVal b:
                sb.Append(b.Value ? "true" : "false");
                break;
            case StringVal s:
                // "<>" is the placeholder the parser emits for an absent value.
                if (s.Value == "<>")
                    sb.Append("<>");
                else
                {
                    sb.Append('"');
                    sb.Append(s.Value);
                    sb.Append('"');
                }
                break;
            case TrimmedString t:
                sb.Append('"');
                sb.Append(t.Value);
                sb.Append('"');
                break;
            case ArrayVal a:
                sb.Append('[');
                var leaves = new List<TestCaseSolutionValue>();
                Flatten(a, leaves);
                RenderList(leaves, sb);
                sb.Append(']');
                break;
            case SetVal set:
                sb.Append('{');
                RenderList(set.Elements, sb);
                sb.Append('}');
                break;
            case RangeVal r:
                RenderValue(r.Lo, sb);
                sb.Append("..");
                RenderValue(r.Hi, sb);
                break;
            case TupleVal tup:
                sb.Append('(');
                RenderList(tup.Fields, sb);
                sb.Append(')');
                break;
            case RecordVal rec:
                sb.Append('(');
                bool firstField = true;
                foreach (var (fieldName, fieldValue) in rec.Fields)
                {
                    if (!firstField)
                        sb.Append(',');
                    firstField = false;
                    sb.Append(fieldName);
                    sb.Append(':');
                    RenderValue(fieldValue, sb);
                }
                sb.Append(')');
                break;
            case EnumVal e:
                sb.Append(e.Constructor);
                if (e.Argument is not null)
                {
                    sb.Append('(');
                    RenderValue(e.Argument, sb);
                    sb.Append(')');
                }
                break;
            case AnonEnumVal anon:
                sb.Append(anon.EnumName);
                sb.Append('(');
                RenderValue(anon.Index, sb);
                sb.Append(')');
                break;
            case ApproxVal approx:
                RenderValue(approx.Inner, sb);
                break;
            case UnorderedVal unordered:
                RenderValue(unordered.Inner, sb);
                break;
            default:
                throw new InvalidOperationException($"cannot render {value.GetType().Name} as DZN");
        }
    }

    /// Flatten nested arrays into a single element list (the integration-test
    /// comparison flattens multi-dimensional arrays anyway).
    private static void Flatten(ArrayVal array, List<TestCaseSolutionValue> leaves)
    {
        foreach (var element in array.Elements)
        {
            if (element is ArrayVal nested)
                Flatten(nested, leaves);
            else
                leaves.Add(element);
        }
    }

    private static void RenderList(IReadOnlyList<TestCaseSolutionValue> values, StringBuilder sb)
    {
        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0)
                sb.Append(',');
            RenderValue(values[i], sb);
        }
    }
}
