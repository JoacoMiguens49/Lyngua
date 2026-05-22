using DuoMusica.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DuoMusica.Services.Spotify;

public class SpotifyWebApiService : ISpotifyWebApiService
{
    private const string ApiBase = "https://api.spotify.com/v1";
    private readonly ISpotifyAuthService _auth;
    private readonly HttpClient _http = new();

    public SpotifyWebApiService(ISpotifyAuthService auth) => _auth = auth;

    public async Task<List<Song>> SearchTracksAsync(string query, string? genre = null, int limit = 20)
    {
        var q = Uri.EscapeDataString(query);
        if (!string.IsNullOrEmpty(genre))
            q += Uri.EscapeDataString($" genre:{genre}");

        var url = $"{ApiBase}/search?q={q}&type=track&limit={limit}";
        var json = await GetAsync(url);
        if (json is null) return new();

        using var doc = JsonDocument.Parse(json);
        var items = doc.RootElement
            .GetProperty("tracks")
            .GetProperty("items");

        var songs = new List<Song>();
        foreach (var item in items.EnumerateArray())
        {
            var song = ParseTrack(item);
            if (song is not null) songs.Add(song);
        }
        return songs;
    }

    public async Task<Song?> GetTrackAsync(string trackId)
    {
        var json = await GetAsync($"{ApiBase}/tracks/{trackId}");
        if (json is null) return null;

        using var doc = JsonDocument.Parse(json);
        return ParseTrack(doc.RootElement);
    }

    public async Task<int?> GetTrackTempoAsync(string trackId)
    {
        var json = await GetAsync($"{ApiBase}/audio-features/{trackId}");
        if (json is null) return null;

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty("tempo", out var tempo))
            return (int)tempo.GetDouble();

        return null;
    }

    private async Task<string?> GetAsync(string url)
    {
        var token = await _auth.GetAccessTokenAsync();
        if (token is null) return null;

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _http.GetAsync(url);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsStringAsync()
            : null;
    }

    private static Song? ParseTrack(JsonElement item)
    {
        if (!item.TryGetProperty("id", out var idEl)) return null;

        var artists = item.GetProperty("artists");
        var artistName = artists.GetArrayLength() > 0
            ? artists[0].GetProperty("name").GetString() ?? string.Empty
            : string.Empty;

        var albumImages = item.GetProperty("album").GetProperty("images");
        var artUrl = albumImages.GetArrayLength() > 0
            ? albumImages[0].GetProperty("url").GetString() ?? string.Empty
            : string.Empty;

        var isrc = string.Empty;
        if (item.TryGetProperty("external_ids", out var extIds) &&
            extIds.TryGetProperty("isrc", out var isrcEl))
            isrc = isrcEl.GetString() ?? string.Empty;

        return new Song
        {
            SpotifyTrackId = idEl.GetString() ?? string.Empty,
            SpotifyUri = item.TryGetProperty("uri", out var uri) ? uri.GetString() ?? string.Empty : string.Empty,
            Name = item.TryGetProperty("name", out var name) ? name.GetString() ?? string.Empty : string.Empty,
            Artist = artistName,
            AlbumName = item.GetProperty("album").GetProperty("name").GetString() ?? string.Empty,
            AlbumArtUrl = artUrl,
            DurationMs = item.TryGetProperty("duration_ms", out var dur) ? dur.GetInt32() : 0,
            Isrc = isrc
        };
    }
}
