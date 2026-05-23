using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lyngua.Models;
using Lyngua.Services.Spotify;
using Lyngua.Views;
using System.Collections.ObjectModel;

namespace Lyngua.ViewModels;

[QueryProperty(nameof(Mode), "Mode")]
public partial class SongSearchViewModel : BaseViewModel
{
    private readonly ISpotifyWebApiService _spotifyApi;
    private readonly ISpotifyAuthService _auth;

    [ObservableProperty] private GameMode _mode;
    [ObservableProperty] private string _searchQuery = string.Empty;
    [ObservableProperty] private Genre? _selectedGenre;
    [ObservableProperty] private bool _isAuthenticated;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public ObservableCollection<Song> Results { get; } = new();
    public IReadOnlyList<Genre> Genres => Genre.All;

    public SongSearchViewModel(ISpotifyWebApiService spotifyApi, ISpotifyAuthService auth)
    {
        _spotifyApi = spotifyApi;
        _auth = auth;
        Title = "Buscar canción";
    }

    public async Task InitializeAsync()
    {
        IsAuthenticated = _auth.IsAuthenticated;
        if (!IsAuthenticated) return;

        await SearchAsync();
    }

    [RelayCommand]
    private async Task AuthenticateAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            IsAuthenticated = await _auth.AuthenticateAsync();
        }
        finally { IsBusy = false; }

        // SearchAsync tiene su propio guard de IsBusy, así que hay que llamarla
        // DESPUÉS de que finally resetee IsBusy, no dentro del try.
        if (IsAuthenticated) await SearchAsync();
    }

    [RelayCommand]
    private async Task SearchAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var query = string.IsNullOrWhiteSpace(SearchQuery) ? "top songs" : SearchQuery;
            var results = await _spotifyApi.SearchTracksAsync(query, SelectedGenre?.SpotifyGenreTag);
            Results.Clear();
            foreach (var song in results) Results.Add(song);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error al buscar. Verificá tu conexión.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private void SelectGenre(Genre genre)
    {
        SelectedGenre = SelectedGenre?.Id == genre.Id ? null : genre;
        SearchAsync().ConfigureAwait(false);
    }

    [RelayCommand]
    private Task SelectSongAsync(Song song) =>
        Shell.Current.GoToAsync(nameof(GamePage), new Dictionary<string, object>
        {
            ["Song"] = song,
            ["Mode"] = Mode
        });
}
