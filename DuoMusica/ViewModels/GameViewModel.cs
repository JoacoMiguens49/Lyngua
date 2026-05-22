using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DuoMusica.Models;
using DuoMusica.Services.Game;
using DuoMusica.Services.Spotify;
using DuoMusica.Views;

namespace DuoMusica.ViewModels;

[QueryProperty(nameof(Song), "Song")]
[QueryProperty(nameof(Mode), "Mode")]
public partial class GameViewModel : BaseViewModel
{
    private readonly IGameService _gameService;
    private readonly ISpotifyRemoteService _remote;
    private GameSession? _session;
    private CancellationTokenSource? _playbackCts;

    [ObservableProperty] private Song? _song;
    [ObservableProperty] private GameMode _mode;
    [ObservableProperty] private SongSegment? _currentSegment;
    [ObservableProperty] private string _userInput = string.Empty;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private double _sessionProgress;
    [ObservableProperty] private int _segmentNumber;
    [ObservableProperty] private int _totalSegments;
    [ObservableProperty] private double _playbackProgress;
    [ObservableProperty] private string _playbackTimeLabel = "0:00";
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _canReplay = true;
    [ObservableProperty] private int _replaysRemaining = 3;

    public GameViewModel(IGameService gameService, ISpotifyRemoteService remote)
    {
        _gameService = gameService;
        _remote = remote;
        Title = "Escuchá y escribí";
    }

    public async Task InitializeAsync()
    {
        if (Song is null) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            await _remote.ConnectAsync();
            _session = await _gameService.BuildSessionAsync(Song, Mode);
            LoadCurrentSegment();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task PlaySegmentAsync()
    {
        if (_session?.CurrentSegment is null || IsPlaying) return;

        if (ReplaysRemaining <= 0) return;
        ReplaysRemaining--;
        CanReplay = ReplaysRemaining > 0;

        IsPlaying = true;
        UserInput = string.Empty;
        _playbackCts?.Cancel();
        _playbackCts = new CancellationTokenSource();

        var segment = _session.CurrentSegment;
        await _remote.PlaySegmentAsync(Song!.SpotifyUri, segment.StartMs, segment.DurationMs);

        // Animate playback progress bar
        _ = AnimatePlaybackAsync(segment.DurationMs, _playbackCts.Token);
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (_session is null || string.IsNullOrWhiteSpace(UserInput)) return;
        IsBusy = true;

        _playbackCts?.Cancel();
        await _remote.PauseAsync();

        try
        {
            var result = await _gameService.EvaluateSegmentAsync(_session, UserInput);
            await _gameService.AdvanceSegmentAsync(_session);

            await Shell.Current.GoToAsync(nameof(ResultsPage), new Dictionary<string, object>
            {
                ["Result"] = result,
                ["Session"] = _session
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void OnReturningFromResults()
    {
        if (_session is null) return;

        if (_session.IsComplete)
        {
            Shell.Current.GoToAsync(nameof(SongCompletePage), new Dictionary<string, object>
            {
                ["Session"] = _session
            });
            return;
        }

        LoadCurrentSegment();
    }

    private void LoadCurrentSegment()
    {
        if (_session is null) return;

        CurrentSegment = _session.CurrentSegment;
        SegmentNumber = _session.CurrentSegmentIndex + 1;
        TotalSegments = _session.Segments.Count;
        SessionProgress = _session.Progress;
        PlaybackProgress = 0;
        PlaybackTimeLabel = "0:00";
        UserInput = string.Empty;
        ReplaysRemaining = 3;
        CanReplay = true;
    }

    private async Task AnimatePlaybackAsync(int durationMs, CancellationToken ct)
    {
        var elapsed = 0;
        const int intervalMs = 100;

        while (elapsed < durationMs && !ct.IsCancellationRequested)
        {
            await Task.Delay(intervalMs, ct).ContinueWith(_ => { });
            elapsed += intervalMs;
            PlaybackProgress = (double)elapsed / durationMs;
            PlaybackTimeLabel = TimeSpan.FromMilliseconds(elapsed).ToString(@"m\:ss");
        }

        IsPlaying = false;
        PlaybackProgress = 1;
    }
}
