using SQLite;

namespace DuoMusica.Models;

[Table("SongProgress")]
public class SongProgress
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }
    public string SpotifyTrackId { get; set; } = string.Empty;
    public string TrackName { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int SegmentsCompleted { get; set; }
    public int TotalSegments { get; set; }
    public double BestScore { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    public bool IsStarted => SegmentsCompleted > 0;
}

[Table("SegmentProgress")]
public class SegmentProgress
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int UserId { get; set; }
    public string SpotifyTrackId { get; set; } = string.Empty;
    public int SegmentIndex { get; set; }
    public double BestScore { get; set; }
    public int AttemptCount { get; set; }
    public DateTime LastAttemptAt { get; set; } = DateTime.UtcNow;
}
