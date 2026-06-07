namespace MiniZinc.TestSpec;

using System.Text;

/// <summary>
/// Extracts YAML preamble document(s) from inside a <c>/*** ... ***/</c>
/// comment at the start of a libminizinc .mzn test file. Multiple documents
/// are separated by <c>---</c>; this method returns each as raw text.
/// </summary>
public static class TestCommentExtractor
{
    /// Returns the raw concatenated YAML text inside the leading /*** ... ***/
    /// block. The opening marker is /*** and the closing is ***/.
    /// If the file has no such comment, returns null.
    public static string? ExtractRawYaml(string source)
    {
        // Find leading /***
        int p = 0;
        // Allow leading whitespace
        while (p < source.Length && char.IsWhiteSpace(source[p]))
            p++;
        if (p + 4 > source.Length)
            return null;
        if (source[p] != '/' || source[p + 1] != '*' || source[p + 2] != '*' || source[p + 3] != '*')
            return null;
        p += 4;
        // Skip until newline (the rest of the opening line — sometimes "/***" is on its own line)
        // Then capture until "***/"
        var sb = new StringBuilder();
        int end = source.Length;
        while (p < end)
        {
            if (p + 3 < end && source[p] == '*' && source[p + 1] == '*' && source[p + 2] == '*' && source[p + 3] == '/')
                break;
            sb.Append(source[p]);
            p++;
        }
        return sb.ToString();
    }
}
