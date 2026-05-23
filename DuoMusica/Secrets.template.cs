namespace Lyngua;

// INSTRUCCIONES DE SETUP:
// 1. Copiá este archivo: Secrets.template.cs → Secrets.cs
// 2. Completá los valores con tus keys reales
// 3. Nunca commitees Secrets.cs (está en .gitignore)
//
// DÓNDE CONSEGUIR LAS KEYS:
// - Spotify Client ID:   https://developer.spotify.com/dashboard → crear app → Settings
//   Redirect URI a registrar: lyngua://callback
// - Musixmatch API Key:  https://developer.musixmatch.com → crear cuenta → API Key

public static partial class AppSettings
{
    static partial void LoadSecrets()
    {
        SpotifyClientId   = "TU_SPOTIFY_CLIENT_ID";
        // MusixmatchApiKey = "TU_MUSIXMATCH_API_KEY";
    }
}
