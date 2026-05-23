using Lyngua.Models;

namespace Lyngua.Helpers;

public static class TextDiffHelper
{
    public static (List<DiffToken> tokens, double score) Compare(string userInput, string correctText)
    {
        var normalizedUser = StringNormalizer.Normalize(userInput);
        var normalizedCorrect = StringNormalizer.Normalize(correctText);

        var userWords = StringNormalizer.Tokenize(normalizedUser);
        var correctWords = StringNormalizer.Tokenize(normalizedCorrect);

        var tokens = new List<DiffToken>();
        double totalPoints = correctWords.Length;
        double earnedPoints = 0;

        for (int i = 0; i < correctWords.Length; i++)
        {
            var correct = correctWords[i];
            var user = i < userWords.Length ? userWords[i] : null;

            if (user is null)
            {
                tokens.Add(new DiffToken { Text = correct, Status = DiffStatus.Missing });
            }
            else if (user == correct)
            {
                tokens.Add(new DiffToken { Text = user, Status = DiffStatus.Correct });
                earnedPoints += 1.0;
            }
            else if (Levenshtein(user, correct) <= 1)
            {
                tokens.Add(new DiffToken { Text = user, Status = DiffStatus.Close });
                earnedPoints += 0.5;
            }
            else
            {
                tokens.Add(new DiffToken { Text = user, Status = DiffStatus.Wrong });
            }
        }

        // Extra words typed by user (no match in correct)
        for (int i = correctWords.Length; i < userWords.Length; i++)
            tokens.Add(new DiffToken { Text = userWords[i], Status = DiffStatus.Wrong });

        var score = totalPoints > 0 ? (earnedPoints / totalPoints) * 100.0 : 0;
        return (tokens, Math.Round(score, 1));
    }

    private static int Levenshtein(string a, string b)
    {
        if (a == b) return 0;
        if (a.Length == 0) return b.Length;
        if (b.Length == 0) return a.Length;

        var dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;

        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1]
                    : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));

        return dp[a.Length, b.Length];
    }
}
