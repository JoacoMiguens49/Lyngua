namespace DuoMusica.Services.Game;

public interface IBotService
{
    void Initialize(double userAverageWpm, double difficultyMultiplier);

    /// <summary>
    /// Returns bot progress as 0.0–1.0 fraction of segment text typed.
    /// </summary>
    double GetProgress(TimeSpan elapsed, int segmentWordCount);

    double BotWpm { get; }
}
