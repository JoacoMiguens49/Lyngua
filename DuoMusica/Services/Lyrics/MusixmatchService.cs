using Lyngua.Helpers;
using Lyngua.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Lyngua.Services.Lyrics;

public class MusixmatchService : ILyricsService
{
    private const string ApiBase = "https://api.musixmatch.com/ws/1.1";
    private readonly HttpClient _http = new();

    public async Task<List<LyricLine>?> GetSyncedLyricsAsync(string trackName, string artistName, string? isrc = null)
    {
        var trackId = await FindTrackIdAsync(trackName, artistName, isrc);
        if (trackId is null) return null;

        return await GetSubtitleAsync(trackId);
    }

    private async Task<string?> FindTrackIdAsync(string trackName, string artistName, string? isrc)
    {
        string url;

        if (!string.IsNullOrEmpty(isrc))
        {
            url = $"{ApiBase}/track.get?track_isrc={Uri.EscapeDataString(isrc)}&apikey={AppSettings.MusixmatchApiKey}";
            var json = await GetAsync(url);
            if (json is not null)
            {
                var id = ExtractTrackId(json);
                if (id is not null) return id;
            }
        }

        url = $"{ApiBase}/track.search" +
              $"?q_track={Uri.EscapeDataString(trackName)}" +
              $"&q_artist={Uri.EscapeDataString(artistName)}" +
              $"&s_track_rating=desc&page_size=1" +
              $"&apikey={AppSettings.MusixmatchApiKey}";

        var searchJson = await GetAsync(url);
        return searchJson is not null ? ExtractTrackId(searchJson, isSearch: true) : null;
    }

    private async Task<List<LyricLine>?> GetSubtitleAsync(string trackId)
    {
        var url = $"{ApiBase}/track.subtitle.get?track_id={trackId}&subtitle_format=lrc&apikey={AppSettings.MusixmatchApiKey}";
        var json = await GetAsync(url);
        if (json is null) return null;

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement.GetProperty("message");

        if (root.GetProperty("header").GetProperty("status_code").GetInt32() != 200)
            return null;

        var subtitleBody = root.GetProperty("body").GetProperty("subtitle").GetProperty("subtitle_body");
        var lrc = subtitleBody.GetString();
        return lrc is null ? null : LrcParser.Parse(lrc);
    }

    private static string? ExtractTrackId(string json, bool isSearch = false)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var body = doc.RootElement.GetProperty("message").GetProperty("body");

            if (isSearch)
            {
                var list = body.GetProperty("track_list");
                if (list.GetArrayLength() == 0) return null;
                return list[0].GetProperty("track").GetProperty("track_id").GetInt32().ToString();
            }

            return body.GetProperty("track").GetProperty("track_id").GetInt32().ToString();
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetAsync(string url)
    {
        try
        {
            var response = await _http.GetAsync(url);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsStringAsync()
                : null;
        }
        catch
        {
            return null;
        }
    }
}
