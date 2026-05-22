using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DuoMusica.Models;
using DuoMusica.Services.User;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DuoMusica.ViewModels;

public partial class OnboardingViewModel : BaseViewModel
{
    private readonly IUserProfileService _userService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStep1))]
    [NotifyPropertyChangedFor(nameof(IsStep2))]
    private int _currentStep = 1; // 1 = native lang, 2 = practice lang

    [ObservableProperty] private string _displayName = string.Empty;

    public bool IsStep1 => CurrentStep == 1;
    public bool IsStep2 => CurrentStep == 2;

    public ObservableCollection<SelectableLanguage> NativeLanguages { get; } = new();
    public ObservableCollection<SelectableLanguage> PracticeLanguages { get; } = new();

    public OnboardingViewModel(IUserProfileService userService)
    {
        _userService = userService;
        Title = "Configurar perfil";

        foreach (var lang in LanguageEntry.Available)
        {
            NativeLanguages.Add(new SelectableLanguage(lang));
            PracticeLanguages.Add(new SelectableLanguage(lang.Code, lang.Name));
        }
    }

    [RelayCommand]
    private void NextStep() => CurrentStep = 2;

    [RelayCommand]
    private async Task FinishAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            var native = NativeLanguages
                .Where(l => l.IsSelected)
                .Select(l => new LanguageEntry { Code = l.Code, Name = l.Name, Level = (LanguageLevel)l.SelectedLevel })
                .ToList();

            var practice = PracticeLanguages
                .Where(l => l.IsSelected)
                .Select(l => new LanguageEntry { Code = l.Code, Name = l.Name, Level = (LanguageLevel)l.SelectedLevel })
                .ToList();

            var profile = new UserProfile
            {
                DisplayName = DisplayName.Trim(),
                NativeLanguagesJson = JsonSerializer.Serialize(native),
                PracticeLanguagesJson = JsonSerializer.Serialize(practice)
            };

            await _userService.SaveProfileAsync(profile);
            await Shell.Current.GoToAsync("//HomePage");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

public partial class SelectableLanguage : ObservableObject
{
    public string Code { get; }
    public string Name { get; }

    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private int _selectedLevel = 0;

    public SelectableLanguage(string code, string name)
    {
        Code = code;
        Name = name;
    }

    public SelectableLanguage(LanguageEntry entry) : this(entry.Code, entry.Name)
    {
        SelectedLevel = (int)entry.Level;
    }

    public string[] LevelOptions => ["Básico", "Intermedio", "Avanzado"];
}
