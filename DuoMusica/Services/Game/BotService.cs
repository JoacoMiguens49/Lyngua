namespace Lyngua.Services.Game;

public class BotService : IBotService
{
    private double _botWpm;
    private static readonly Random _rng = new();

    public double BotWpm => _botWpm;

    public void Initialize(double userAverageWpm, double difficultyMultiplier)
    {
        // Introduce ±15% variance to make the bot feel natural
        var variance = 1.0 + (_rng.NextDouble() * 0.3 - 0.15);
        _botWpm = Math.Max(10, userAverageWpm * difficultyMultiplier * variance);
    }

    public double GetProgress(TimeSpan elapsed, int segmentWordCount)
    {
        if (segmentWordCount <= 0) return 0;

        var wordsTyped = _botWpm * (elapsed.TotalSeconds / 60.0);
        return Math.Min(1.0, wordsTyped / segmentWordCount);
    }
}
