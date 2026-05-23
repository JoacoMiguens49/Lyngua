using Lyngua.Models;

namespace Lyngua.Services.Lyrics;

public interface ILyricsService
{
    Task<List<LyricLine>?> GetSyncedLyricsAsync(string trackName, string artistName, string? isrc = null);
}
