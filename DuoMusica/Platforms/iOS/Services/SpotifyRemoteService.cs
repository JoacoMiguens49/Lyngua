using DuoMusica.Services.Spotify;

namespace DuoMusica.Platforms.iOS.Services;

/// <summary>
/// iOS implementation using Spotify iOS SDK (SpotifyiOS.xcframework).
///
/// SETUP REQUIRED:
/// 1. Download SpotifyiOS.xcframework from https://github.com/spotify/ios-sdk
/// 2. Create a .NET iOS Binding project referencing the XCFramework
/// 3. Reference the binding from DuoMusica
/// 4. Add the custom URL scheme (your redirect URI) to Info.plist
/// 5. Implement SPTAppRemoteDelegate
///
/// Once bindings are in place, replace the stub body below with:
///   - SPTAppRemote.authorize(configuration:) to open Spotify
///   - appRemote.connect() / appRemote.playerAPI.play(uri)
///   - appRemote.playerAPI.seek(toPosition:)
///   - Stop via Timer after durationMs
/// </summary>
public class SpotifyRemoteService : ISpotifyRemoteService
{
    // TODO: Replace with real SPTAppRemote instance once binding is added
    // private SPTAppRemote? _appRemote;

    public Task<bool> ConnectAsync()
    {
        // TODO: _appRemote.Connect()
        return Task.FromResult(false);
    }

    public Task PlaySegmentAsync(string spotifyUri, int startMs, int durationMs)
    {
        // TODO: _appRemote.PlayerAPI.Play(spotifyUri, ...)
        //       _appRemote.PlayerAPI.Seek(startMs)
        //       Start a timer to pause after durationMs
        return Task.CompletedTask;
    }

    public Task PauseAsync()
    {
        // TODO: _appRemote.PlayerAPI.Pause()
        return Task.CompletedTask;
    }

    public Task SeekToAsync(int positionMs)
    {
        // TODO: _appRemote.PlayerAPI.Seek(positionMs)
        return Task.CompletedTask;
    }

    public Task<bool> IsConnectedAsync()
    {
        // TODO: return _appRemote?.IsConnected ?? false
        return Task.FromResult(false);
    }
}
