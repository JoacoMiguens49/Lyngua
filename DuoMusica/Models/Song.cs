namespace DuoMusica.Models;

public class Song
{
    public string SpotifyTrackId { get; set; } = string.Empty;
    public string SpotifyUri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string AlbumName { get; set; } = string.Empty;
    public string AlbumArtUrl { get; set; } = string.Empty;
    public int DurationMs { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int TempoBeatsPerMinute { get; set; }
    public DifficultyLevel Difficulty { get; set; }
    public string Isrc { get; set; } = string.Empty;

    public string DurationFormatted =>
        TimeSpan.FromMilliseconds(DurationMs).ToString(@"m\:ss");
}
