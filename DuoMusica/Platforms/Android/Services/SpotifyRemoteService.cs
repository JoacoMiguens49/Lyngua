using Android.Content;
using Com.Spotify.Android.Appremote.Api;
using Lyngua.Services.Spotify;

namespace Lyngua.Platforms.Android.Services;

/// <summary>
/// Implementación real del Spotify App Remote SDK para Android.
/// Controla la app de Spotify instalada en el dispositivo — no requiere Premium.
/// El AAR se incluye en Platforms/Android/Libs/ y los bindings se generan con Bind="true".
/// </summary>
public class SpotifyRemoteService : Java.Lang.Object,
    ISpotifyRemoteService,
    IConnector.IConnectionListener
{
    private SpotifyAppRemote? _remote;
    private TaskCompletionSource<bool>? _connectTcs;
    private CancellationTokenSource? _segmentCts;

    // ── ISpotifyRemoteService ────────────────────────────────────────────────

    public async Task<bool> ConnectAsync()
    {
        if (_remote?.IsConnected == true) return true;

        _connectTcs = new TaskCompletionSource<bool>();

        var context = global::Android.App.Application.Context;
        var @params = new ConnectionParams.Builder(AppSettings.SpotifyClientId)
            .SetRedirectUri(AppSettings.SpotifyRedirectUri)
            .ShowAuthView(true)
            .Build();

        SpotifyAppRemote.Connect(context, @params, this);

        // Timeout de 10 segundos
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        timeout.Token.Register(() => _connectTcs.TrySetResult(false));

        return await _connectTcs.Task;
    }

    public async Task PlaySegmentAsync(string spotifyUri, int startMs, int durationMs)
    {
        if (_remote?.IsConnected != true)
        {
            if (!await ConnectAsync()) return;
        }

        var player = _remote!.PlayerApi;
        if (player is null) return;

        // Cancela cualquier segmento que estuviera corriendo
        _segmentCts?.Cancel();
        _segmentCts = new CancellationTokenSource();
        var token = _segmentCts.Token;

        // Play → Seek inmediato al punto de inicio → esperar duración → Pause
        player.Play(spotifyUri);
        player.SeekTo((long)startMs);

        try
        {
            await Task.Delay(durationMs, token);
            player.Pause();
        }
        catch (TaskCanceledException)
        {
            // Segmento interrumpido por el usuario, no es un error
        }
    }

    public Task PauseAsync()
    {
        _segmentCts?.Cancel();
        _remote?.PlayerApi?.Pause();
        return Task.CompletedTask;
    }

    public Task SeekToAsync(int positionMs)
    {
        _remote?.PlayerApi?.SeekTo((long)positionMs);
        return Task.CompletedTask;
    }

    public Task<bool> IsConnectedAsync() =>
        Task.FromResult(_remote?.IsConnected == true);

    // ── IConnector.IConnectionListener (callbacks del SDK) ──────────────────

    public void OnConnected(SpotifyAppRemote? remote)
    {
        _remote = remote;
        _connectTcs?.TrySetResult(true);
    }

    public void OnFailure(Java.Lang.Throwable? throwable)
    {
        System.Diagnostics.Debug.WriteLine(
            $"[Spotify Remote] Falló la conexión: {throwable?.Message}");
        _remote = null;
        _connectTcs?.TrySetResult(false);
    }
}
