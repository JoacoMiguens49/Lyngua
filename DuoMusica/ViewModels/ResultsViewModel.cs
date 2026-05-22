using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DuoMusica.Models;
using DuoMusica.Views;
using System.Collections.ObjectModel;

namespace DuoMusica.ViewModels;

[QueryProperty(nameof(Result), "Result")]
[QueryProperty(nameof(Session), "Session")]
public partial class ResultsViewModel : BaseViewModel
{
    [ObservableProperty] private SegmentResult? _result;
    [ObservableProperty] private GameSession? _session;
    [ObservableProperty] private string _scoreText = "0%";
    [ObservableProperty] private Color _scoreColor = Colors.White;
    [ObservableProperty] private string _xpText = "+0 XP";
    [ObservableProperty] private bool _isLastSegment;

    public ObservableCollection<DiffToken> DiffTokens { get; } = new();

    public void OnAppearing()
    {
        if (Result is null) return;

        ScoreText = $"{Result.Score:0}%";
        XpText = $"+{Result.XpEarned} XP";
        IsLastSegment = Session?.IsComplete ?? false;

        ScoreColor = Result.Score switch
        {
            >= 90 => Color.FromArgb("#00B894"),
            >= 60 => Color.FromArgb("#FDCB6E"),
            _ => Color.FromArgb("#D63031")
        };

        DiffTokens.Clear();
        if (Result.DiffTokens is not null)
            foreach (var t in Result.DiffTokens) DiffTokens.Add(t);
    }

    [RelayCommand]
    private async Task NextSegmentAsync()
    {
        // Navigate back to GamePage — GameViewModel.OnReturningFromResults handles routing
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task RepeatSegmentAsync()
    {
        // Pop back to game without advancing
        await Shell.Current.GoToAsync("..");
    }
}
