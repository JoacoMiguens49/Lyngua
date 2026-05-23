using Lyngua.Helpers;
using Lyngua.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Lyngua.Services.Lyrics;

/// <summary>
/// Implementación de letras sincronizadas usando lrclib.net.
/// Gratuito, sin API key, open source. Formato LRC con timestamps.
/// Docs: https://lrclib.net/docs
/// </summary>
public class LrcLibService : ILyricsService
{
    private const string ApiBase = "https://lrclib.net/api";
    private static readonly HttpClient _http = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "Lyngua/1.0 (https://github.com/lyngua)" } }
    };

    public async Task<List<LyricLine>?> GetSyncedLyricsAsync(
        string trackName, string artistName, string? isrc = null, int durationMs = 0)
    {
        // Intento 1: match exacto con duración (más preciso)
        if (durationMs > 0)
        {
            var exact = await GetExactAsync(trackName, artistName, durationMs);
            if (exact is not null) return exact;
        }

        // Intento 2: búsqueda por nombre + artista, toma el primer resultado con synced lyrics
        return await SearchAsync(trackName, artistName);
    }

    // GET /api/get?track_name=...&artist_name=...&duration=...
    private async Task<List<LyricLine>?> GetExactAsync(string track, string artist, int durationMs)
    {
        var durationSecs = durationMs / 1000.0;
        var url = $"{ApiBase}/get" +
                  $"?track_name={Uri.EscapeDataString(track)}" +
                  $"&artist_name={Uri.EscapeDataString(artist)}" +
                  $"&duration={durationSecs:F0}";
        try
        {
            var result = await _http.GetFromJsonAsync<LrcLibTrack>(url);
            return ParseSynced(result);
        }
        catch { return null; }
    }

    // GET /api/search?track_name=...&artist_name=...
    private async Task<List<LyricLine>?> SearchAsync(string track, string artist)
    {
        var url = $"{ApiBase}/search" +
                  $"?track_name={Uri.EscapeDataString(track)}" +
                  $"&artist_name={Uri.EscapeDataString(artist)}";
        try
        {
            var results = await _http.GetFromJsonAsync<List<LrcLibTrack>>(url);
            if (results is null) return null;

            // Toma el primer resultado que tenga letras sincronizadas
            foreach (var r in results)
            {
                var parsed = ParseSynced(r);
                if (parsed is { Count: > 0 }) return parsed;
            }
            return null;
        }
        catch { return null; }
    }

    private static List<LyricLine>? ParseSynced(LrcLibTrack? track)
    {
        if (track is null || string.IsNullOrWhiteSpace(track.SyncedLyrics))
            return null;

        var lines = LrcParser.Parse(track.SyncedLyrics);
        return lines.Count > 0 ? lines : null;
    }

    private sealed class LrcLibTrack
    {
        [JsonPropertyName("trackName")]   public string TrackName { get; init; } = "";
        [JsonPropertyName("artistName")]  public string ArtistName { get; init; } = "";
        [JsonPropertyName("duration")]    public double Duration { get; init; }
        [JsonPropertyName("instrumental")]public bool Instrumental { get; init; }
        [JsonPropertyName("syncedLyrics")]public string? SyncedLyrics { get; init; }
        [JsonPropertyName("plainLyrics")] public string? PlainLyrics { get; init; }
    }
}
