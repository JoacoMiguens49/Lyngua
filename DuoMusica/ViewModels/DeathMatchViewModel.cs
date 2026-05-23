using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lyngua.Models;
using Lyngua.Services.Game;
using Lyngua.Services.Spotify;
using Lyngua.Services.User;
using Lyngua.Views;

namespace Lyngua.ViewModels;

[QueryProperty(nameof(Song), "Song")]
public partial class DeathMatchViewModel : BaseViewModel
{
    private readonly IGameService _gameService;
    private readonly ISpotifyRemoteService _remote;
    private readonly IBotService _bot;
    private readonly IUserProfileService _userService;

    private GameSession? _session;
    private DateTime _segmentStartTime;
    private System.Timers.Timer? _botTimer;
    private CancellationTokenSource? _playbackCts;

    [ObservableProperty] private Song? _song;
    [ObservableProperty] private SongSegment? _currentSegment;
    [ObservableProperty] private string _userInput = string.Empty;
    [ObservableProperty] private double _userProgress;
    [ObservableProperty] private double _botProgress;
    [ObservableProperty] private bool _isPlaying;
    [ObservableProperty] private string _statusText = "Preparate...";
    [ObservableProperty] private bool _matchEnded;
    [ObservableProperty] private bool _userWon;
    [ObservableProperty] private string _resultText = string.Empty;

    // Difficulty multipliers
    private const double EasyMultiplier = 0.7;
    private const double MediumMultiplier = 1.0;
    private const double HardMultiplier = 1.3;

    public DeathMatchViewModel(
        IGameService gameService,
        ISpotifyRemoteService remote,
        IBotService bot,
        IUserProfileService userService)
    {
        _gameService = gameService;
        _remote = remote;
        _bot = bot;
        _userService = userService;
        Title = "Deathmatch";
    }

    public async Task InitializeAsync()
    {
        if (Song is null) return;
        IsBusy = true;

        try
        {
            var profile = await _userService.GetProfileAsync();
            var difficulty = Song.Difficulty switch
            {
                DifficultyLevel.Beginner => EasyMultiplier,
                DifficultyLevel.Advanced => HardMultiplier,
                _ => MediumMultiplier
            };

            _bot.Initialize(profile?.AverageWpm ?? 40, difficulty);
            await _remote.ConnectAsync();
            _session = await _gameService.BuildSessionAsync(Song, GameMode.DeathMatch);
            LoadCurrentSegment();
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

        IsPlaying = true;
        StatusText = "¡Escribí!";
        UserInput = string.Empty;
        UserProgress = 0;
        BotProgress = 0;
        _segmentStartTime = DateTime.UtcNow;

        var segment = _session.CurrentSegment;
        await _remote.PlaySegmentAsync(Song!.SpotifyUri, segment.StartMs, segment.DurationMs);

        StartBotTimer(segment);
    }

    partial void OnUserInputChanged(string value)
    {
        if (_session?.CurrentSegment is null) return;
        var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        var totalWords = _session.CurrentSegment.FullText
            .Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        UserProgress = totalWords > 0 ? Math.Min(1.0, (double)words / totalWords) : 0;
    }

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (_session is null) return;

        _botTimer?.Stop();
        _playbackCts?.Cancel();
        await _remote.PauseAsync();
        IsPlaying = false;

        var result = await _gameService.EvaluateSegmentAsync(_session, UserInput);
        await _gameService.AdvanceSegmentAsync(_session);

        var botFinalProgress = _bot.GetProgress(DateTime.UtcNow - _segmentStartTime,
            _session.CurrentSegment?.FullText.Split(' ').Length ?? 1);

        MatchEnded = true;
        UserWon = result.Score / 100.0 >= botFinalProgress;
        ResultText = UserWon
            ? $"¡Ganaste! {result.Score:0}% vs {botFinalProgress * 100:0}%"
            : $"El bot ganó. {result.Score:0}% vs {botFinalProgress * 100:0}%";
    }

    [RelayCommand]
    private async Task NextRoundAsync()
    {
        MatchEnded = false;
        if (_session?.IsComplete == true)
        {
            await Shell.Current.GoToAsync(nameof(SongCompletePage), new Dictionary<string, object>
            {
                ["Session"] = _session
            });
            return;
        }
        LoadCurrentSegment();
    }

    [RelayCommand]
    private Task GoHomeAsync() => Shell.Current.GoToAsync("//HomePage");

    private void StartBotTimer(SongSegment segment)
    {
        _botTimer?.Dispose();
        _botTimer = new System.Timers.Timer(200);
        _botTimer.Elapsed += (_, _) =>
        {
            var elapsed = DateTime.UtcNow - _segmentStartTime;
            var wordCount = segment.FullText.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
            BotProgress = _bot.GetProgress(elapsed, wordCount);

            if (BotProgress >= 1.0)
            {
                _botTimer.Stop();
                MainThread.BeginInvokeOnMainThread(() => StatusText = "¡El bot terminó!");
            }
        };
        _botTimer.Start();
    }

    private void LoadCurrentSegment()
    {
        CurrentSegment = _session?.CurrentSegment;
        UserInput = string.Empty;
        UserProgress = 0;
        BotProgress = 0;
        StatusText = "Tocá Play para escuchar";
        IsPlaying = false;
    }
}
