using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DuoMusica.Helpers;

public static class StringNormalizer
{
    private static readonly Regex _whitespace = new(@"\s+", RegexOptions.Compiled);
    private static readonly Regex _punctuation = new(@"[^\w\s]", RegexOptions.Compiled);

    public static string Normalize(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var lower = input.ToLowerInvariant();
        var noPunctuation = _punctuation.Replace(lower, " ");
        var normalized = RemoveDiacritics(noPunctuation);
        return _whitespace.Replace(normalized, " ").Trim();
    }

    public static string[] Tokenize(string normalized) =>
        normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    private static string RemoveDiacritics(string text)
    {
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(normalizedString.Length);
        foreach (var c in normalizedString)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
