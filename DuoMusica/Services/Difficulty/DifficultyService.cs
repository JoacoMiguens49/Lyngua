using Lyngua.Models;

namespace Lyngua.Services.Difficulty;

public class DifficultyService : IDifficultyService
{
    public DifficultyLevel Calculate(List<SongSegment> segments, int tempoBeatsPerMinute)
    {
        if (segments.Count == 0) return DifficultyLevel.Intermediate;

        var allWords = segments
            .SelectMany(s => s.Lines)
            .SelectMany(l => l.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(w => w.ToLowerInvariant())
            .ToList();

        if (allWords.Count == 0) return DifficultyLevel.Intermediate;

        var wordCount = allWords.Count;
        var avgWordLength = allWords.Average(w => w.Length);
        var uniqueRatio = (double)allWords.Distinct().Count() / wordCount;
        var tempoFactor = tempoBeatsPerMinute > 0 ? tempoBeatsPerMinute / 200.0 : 0.5;

        // Score 0-100
        var score = wordCount * 0.3
                  + avgWordLength * 5 * 0.2
                  + uniqueRatio * 100 * 0.3
                  + tempoFactor * 100 * 0.2;

        return score switch
        {
            < 30 => DifficultyLevel.Beginner,
            < 60 => DifficultyLevel.Intermediate,
            _ => DifficultyLevel.Advanced
        };
    }
}
