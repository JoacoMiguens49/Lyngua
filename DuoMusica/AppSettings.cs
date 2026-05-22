namespace DuoMusica;

public static class AppSettings
{
    public static string SpotifyClientId { get; set; } = string.Empty;
    public static string SpotifyRedirectUri { get; set; } = "duomusica://callback";
    public static string MusixmatchApiKey { get; set; } = string.Empty;

    public static void Load()
    {
        // Set your API keys here or load from secure config
        // SpotifyClientId = "YOUR_SPOTIFY_CLIENT_ID";
        // MusixmatchApiKey = "YOUR_MUSIXMATCH_API_KEY";
    }
}
