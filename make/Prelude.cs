namespace Make;

using System.Text;

public static class Prelude
{
    private static readonly char[] separator = new char[] { ' ', '-', '_', '.' };

    public static string ToCamelCase(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        for (int i = 0; i < words.Length; i++)
        {
            if (i == 0)
                result.Append(words[i].ToLower());
            else
                result.Append(char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower());
        }

        return result.ToString();
    }

    public static string ToClassName(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var words = input.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var result = new StringBuilder();

        for (int i = 0; i < words.Length; i++)
            result.Append(char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower());

        return result.ToString();
    }
}
