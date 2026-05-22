using DuoMusica.Models;

namespace DuoMusica.Services.Spotify;

public interface ISpotifyWebApiService
{
    Task<List<Song>> SearchTracksAsync(string query, string? genre = null, int limit = 20);
    Task<Song?> GetTrackAsync(string trackId);
    Task<int?> GetTrackTempoAsync(string trackId);
}
