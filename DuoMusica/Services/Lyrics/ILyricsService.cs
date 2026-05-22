using DuoMusica.Models;

namespace DuoMusica.Services.Lyrics;

public interface ILyricsService
{
    Task<List<LyricLine>?> GetSyncedLyricsAsync(string trackName, string artistName, string? isrc = null);
}
