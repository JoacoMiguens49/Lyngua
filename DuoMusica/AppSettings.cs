namespace Lyngua;

public static partial class AppSettings
{
    public static string SpotifyClientId { get; set; } = string.Empty;
    public static string SpotifyRedirectUri { get; set; } = "lyngua://callback";
    public static string MusixmatchApiKey { get; set; } = string.Empty;

    public static void Load()
    {
        // Keys are injected by Secrets.cs (gitignored).
        // If that file doesn't exist the app runs with empty keys (safe default).
        LoadSecrets();
    }

    static partial void LoadSecrets();
}
