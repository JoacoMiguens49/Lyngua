using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lyngua.Data.Repositories;
using Lyngua.Helpers;
using Lyngua.Models;
using Lyngua.Services.User;
using Lyngua.Views;
using System.Collections.ObjectModel;

namespace Lyngua.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly IUserProfileService _userService;
    private readonly ProgressRepository _progress;

    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private int _level = 1;
    [ObservableProperty] private int _totalXp;
    [ObservableProperty] private double _levelProgress;
    [ObservableProperty] private string _levelProgressLabel = string.Empty;
    [ObservableProperty] private int _songsCompleted;

    public ObservableCollection<SongProgress> RecentSongs { get; } = new();

    public HomeViewModel(IUserProfileService userService, ProgressRepository progress)
    {
        _userService = userService;
        _progress = progress;
        Title = "Lyngua";
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var profile = await _userService.GetProfileAsync();
            if (profile is null)
            {
                await Shell.Current.GoToAsync("//OnboardingPage");
                return;
            }

            DisplayName = string.IsNullOrEmpty(profile.DisplayName) ? "Jugador" : profile.DisplayName;
            Level = profile.Level;
            TotalXp = profile.TotalXp;
            SongsCompleted = profile.SongsCompleted;

            var (current, required) = XpCalculator.GetLevelProgress(TotalXp);
            LevelProgress = required > 0 ? (double)current / required : 0;
            LevelProgressLabel = $"{current} / {required} XP";

            var recent = await _progress.GetRecentAsync(profile.Id, 5);
            RecentSongs.Clear();
            foreach (var s in recent) RecentSongs.Add(s);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task NavigateToClassicAsync() =>
        Shell.Current.GoToAsync(nameof(SongSearchPage), new Dictionary<string, object>
        {
            ["Mode"] = GameMode.Classic
        });

    [RelayCommand]
    private Task NavigateToDeathMatchAsync() =>
        Shell.Current.GoToAsync(nameof(SongSearchPage), new Dictionary<string, object>
        {
            ["Mode"] = GameMode.DeathMatch
        });

    [RelayCommand]
    private Task ContinueSongAsync(SongProgress song) =>
        Shell.Current.GoToAsync(nameof(GamePage), new Dictionary<string, object>
        {
            ["TrackId"] = song.SpotifyTrackId,
            ["Mode"] = GameMode.Classic
        });
}
