using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lyngua.Models;
using Lyngua.Services.Game;
using Lyngua.Views;

namespace Lyngua.ViewModels;

[QueryProperty(nameof(Session), "Session")]
public partial class SongCompleteViewModel : BaseViewModel
{
    private readonly IGameService _gameService;

    [ObservableProperty] private GameSession? _session;
    [ObservableProperty] private string _songName = string.Empty;
    [ObservableProperty] private string _totalScoreText = "0%";
    [ObservableProperty] private string _totalXpText = "+0 XP";
    [ObservableProperty] private int _segmentsCompleted;
    [ObservableProperty] private bool _isPerfect;

    public SongCompleteViewModel(IGameService gameService)
    {
        _gameService = gameService;
        Title = "¡Canción completada!";
    }

    public async Task OnAppearingAsync()
    {
        if (Session is null) return;

        await _gameService.FinalizeSessionAsync(Session);

        SongName = Session.Song.Name;
        SegmentsCompleted = Session.Segments.Count;
        TotalScoreText = $"{Session.TotalScore:0}%";
        TotalXpText = $"+{Session.TotalXpEarned} XP";
        IsPerfect = Session.TotalScore >= 98;
    }

    [RelayCommand]
    private async Task PlayAgainAsync()
    {
        if (Session is null) return;
        await Shell.Current.GoToAsync(nameof(GamePage), new Dictionary<string, object>
        {
            ["Song"] = Session.Song,
            ["Mode"] = Session.Mode
        });
    }

    [RelayCommand]
    private Task GoHomeAsync() => Shell.Current.GoToAsync("//HomePage");
}
