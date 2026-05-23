using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace Lyngua;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode |
                           ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "lyngua")]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnResume()
    {
        base.OnResume();
        // Forward OAuth callback to WebAuthenticator
        if (Intent?.Data?.Scheme == "lyngua")
            Platform.OnResume(this);
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        // Handle callback when activity is already running
        if (intent?.Data?.Scheme == "lyngua")
        {
            Intent = intent;
            Platform.OnNewIntent(intent);
        }
    }
}
