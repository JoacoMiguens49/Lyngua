namespace Lyngua.Services.Spotify;

/// <summary>
/// Stub for non-mobile platforms (Windows dev builds). Simulates Spotify Remote behavior.
/// </summary>
public class SpotifyRemoteServiceStub : ISpotifyRemoteService
{
    public Task<bool> ConnectAsync() => Task.FromResult(true);
    public Task PlaySegmentAsync(string spotifyUri, int startMs, int durationMs) => Task.CompletedTask;
    public Task PauseAsync() => Task.CompletedTask;
    public Task SeekToAsync(int positionMs) => Task.CompletedTask;
    public Task<bool> IsConnectedAsync() => Task.FromResult(true);
}
