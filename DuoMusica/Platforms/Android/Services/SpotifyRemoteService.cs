using DuoMusica.Services.Spotify;

namespace DuoMusica.Platforms.Android.Services;

/// <summary>
/// Android implementation using Spotify App Remote SDK.
///
/// SETUP REQUIRED:
/// 1. Download spotify-app-remote-release-X.X.X.aar from https://github.com/spotify/android-sdk
/// 2. Create a new Android Binding Library project (DuoMusica.Android.SpotifyBinding)
/// 3. Add the AAR as a native library in the binding project
/// 4. Reference the binding project from DuoMusica
/// 5. Register your redirect URI in the Spotify Developer Dashboard
///
/// Once bindings are in place, replace the stub body below with:
///   - SpotifyAppRemote.Connect(context, connectionParams, connectionListener)
///   - _remote.PlayerApi.Play(spotifyUri)
///   - _remote.PlayerApi.SeekTo(startMs)
///   - Stop playback via a Timer after durationMs
/// </summary>
public class SpotifyRemoteService : ISpotifyRemoteService
{
    // TODO: Replace with real Spotify App Remote instance once binding is added
    // private SpotifyAppRemote? _remote;

    public Task<bool> ConnectAsync()
    {
        // TODO: SpotifyAppRemote.Connect(...)
        return Task.FromResult(false);
    }

    public Task PlaySegmentAsync(string spotifyUri, int startMs, int durationMs)
    {
        // TODO: _remote.PlayerApi.Play(spotifyUri)
        //       _remote.PlayerApi.SeekTo(startMs)
        //       Start a timer to pause after durationMs
        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        // TODO: _remote.PlayerApi.Pause()
        return Task.CompletedTask;
    }

    public Task SeekToAsync(int positionMs)
    {
        // TODO: _remote.PlayerApi.SeekTo(positionMs)
        return Task.CompletedTask;
    }

    public Task<bool> IsConnectedAsync()
    {
        // TODO: return _remote?.IsConnected ?? false
        return Task.FromResult(false);
    }
}
