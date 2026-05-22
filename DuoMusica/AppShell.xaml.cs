using DuoMusica.Views;

namespace DuoMusica;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(OnboardingPage), typeof(OnboardingPage));
        Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
        Routing.RegisterRoute(nameof(SongSearchPage), typeof(SongSearchPage));
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
        Routing.RegisterRoute(nameof(ResultsPage), typeof(ResultsPage));
        Routing.RegisterRoute(nameof(SongCompletePage), typeof(SongCompletePage));
        Routing.RegisterRoute(nameof(DeathMatchPage), typeof(DeathMatchPage));
    }
}
