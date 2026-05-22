namespace DuoMusica.Models;

public enum GameMode
{
    Classic,
    DeathMatch
}

public class GameSession
{
    public string SessionId { get; set; } = Guid.NewGuid().ToString();
    public Song Song { get; set; } = null!;
    public GameMode Mode { get; set; }
    public List<SongSegment> Segments { get; set; } = new();
    public int CurrentSegmentIndex { get; set; }
    public List<SegmentResult> Results { get; set; } = new();
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public SongSegment? CurrentSegment =>
        CurrentSegmentIndex < Segments.Count ? Segments[CurrentSegmentIndex] : null;

    public bool IsComplete => CurrentSegmentIndex >= Segments.Count;

    public double TotalScore => Results.Count > 0
        ? Results.Average(r => r.Score)
        : 0;

    public int TotalXpEarned => Results.Sum(r => r.XpEarned);

    public double Progress => Segments.Count > 0
        ? (double)CurrentSegmentIndex / Segments.Count
        : 0;
}
