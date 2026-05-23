using CommunityToolkit.Maui;
using Lyngua.Data;
using Lyngua.Data.Repositories;
using Lyngua.Services.Difficulty;
using Lyngua.Services.Game;
using Lyngua.Services.Lyrics;
using Lyngua.Services.Spotify;
using Lyngua.Services.User;
using Lyngua.ViewModels;
using Lyngua.Views;
using Microsoft.Extensions.Logging;

namespace Lyngua;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        AppSettings.Load();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Data
        builder.Services.AddSingleton<AppDatabase>();
        builder.Services.AddSingleton<UserRepository>();
        builder.Services.AddSingleton<ProgressRepository>();

        // Services
        builder.Services.AddSingleton<ISpotifyAuthService, SpotifyAuthService>();
        builder.Services.AddSingleton<ISpotifyWebApiService, SpotifyWebApiService>();
        builder.Services.AddSingleton<ILyricsService, MusixmatchService>();
        builder.Services.AddSingleton<IUserProfileService, UserProfileService>();
        builder.Services.AddSingleton<IDifficultyService, DifficultyService>();
        builder.Services.AddSingleton<IGameService, GameService>();
        builder.Services.AddTransient<IBotService, BotService>();

#if ANDROID
        builder.Services.AddSingleton<ISpotifyRemoteService, Lyngua.Platforms.Android.Services.SpotifyRemoteService>();
#elif IOS
        builder.Services.AddSingleton<ISpotifyRemoteService, Lyngua.Platforms.iOS.Services.SpotifyRemoteService>();
#else
        builder.Services.AddSingleton<ISpotifyRemoteService, SpotifyRemoteServiceStub>();
#endif

        // ViewModels
        builder.Services.AddTransient<OnboardingViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<SongSearchViewModel>();
        builder.Services.AddTransient<GameViewModel>();
        builder.Services.AddTransient<ResultsViewModel>();
        builder.Services.AddTransient<SongCompleteViewModel>();
        builder.Services.AddTransient<DeathMatchViewModel>();

        // Views
        builder.Services.AddTransient<OnboardingPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<SongSearchPage>();
        builder.Services.AddTransient<GamePage>();
        builder.Services.AddTransient<ResultsPage>();
        builder.Services.AddTransient<SongCompletePage>();
        builder.Services.AddTransient<DeathMatchPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
