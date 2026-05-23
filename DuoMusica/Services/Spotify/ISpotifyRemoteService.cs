namespace Lyngua.Services.Spotify;

public interface ISpotifyRemoteService
{
    Task<bool> ConnectAsync();
    Task PlaySegmentAsync(string spotifyUri, int startMs, int durationMs);
    Task PauseAsync();
    Task SeekToAsync(int positionMs);
    Task<bool> IsConnectedAsync();
}
