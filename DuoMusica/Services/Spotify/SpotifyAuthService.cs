using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Lyngua.Services.Spotify;

public class SpotifyAuthService : ISpotifyAuthService
{
    private const string TokenKey = "spotify_access_token";
    private const string RefreshTokenKey = "spotify_refresh_token";
    private const string TokenExpiryKey = "spotify_token_expiry";
    private const string AuthEndpoint = "https://accounts.spotify.com/authorize";
    private const string TokenEndpoint = "https://accounts.spotify.com/api/token";
    private const string Scopes = "user-modify-playback-state app-remote-control streaming user-read-playback-state";

    private string? _codeVerifier;

    public bool IsAuthenticated =>
        SecureStorage.Default.GetAsync(TokenKey).Result is not null;

    public async Task<bool> AuthenticateAsync()
    {
        _codeVerifier = GenerateCodeVerifier();
        var codeChallenge = GenerateCodeChallenge(_codeVerifier);

        var authUrl = $"{AuthEndpoint}" +
            $"?client_id={Uri.EscapeDataString(AppSettings.SpotifyClientId)}" +
            $"&response_type=code" +
            $"&redirect_uri={Uri.EscapeDataString(AppSettings.SpotifyRedirectUri)}" +
            $"&code_challenge_method=S256" +
            $"&code_challenge={codeChallenge}" +
            $"&scope={Uri.EscapeDataString(Scopes)}";

        var result = await WebAuthenticator.Default.AuthenticateAsync(
            new Uri(authUrl),
            new Uri(AppSettings.SpotifyRedirectUri));

        if (!result.Properties.TryGetValue("code", out var code))
            return false;

        return await ExchangeCodeAsync(code);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var token = await SecureStorage.Default.GetAsync(TokenKey);
        var expiryStr = await SecureStorage.Default.GetAsync(TokenExpiryKey);

        if (token is not null && expiryStr is not null)
        {
            if (DateTimeOffset.TryParse(expiryStr, out var expiry) && expiry > DateTimeOffset.UtcNow.AddMinutes(1))
                return token;
        }

        // Token expired — refresh
        var refreshToken = await SecureStorage.Default.GetAsync(RefreshTokenKey);
        if (refreshToken is null) return null;

        return await RefreshTokenAsync(refreshToken);
    }

    public async Task RevokeAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(RefreshTokenKey);
        SecureStorage.Default.Remove(TokenExpiryKey);
    }

    private async Task<bool> ExchangeCodeAsync(string code)
    {
        using var http = new HttpClient();
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = AppSettings.SpotifyRedirectUri,
            ["client_id"] = AppSettings.SpotifyClientId,
            ["code_verifier"] = _codeVerifier!
        });

        var response = await http.PostAsync(TokenEndpoint, body);
        if (!response.IsSuccessStatusCode) return false;

        return await ParseAndStoreTokensAsync(await response.Content.ReadAsStringAsync());
    }

    private async Task<string?> RefreshTokenAsync(string refreshToken)
    {
        using var http = new HttpClient();
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = refreshToken,
            ["client_id"] = AppSettings.SpotifyClientId
        });

        var response = await http.PostAsync(TokenEndpoint, body);
        if (!response.IsSuccessStatusCode) return null;

        if (!await ParseAndStoreTokensAsync(await response.Content.ReadAsStringAsync()))
            return null;

        return await SecureStorage.Default.GetAsync(TokenKey);
    }

    private async Task<bool> ParseAndStoreTokensAsync(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("access_token", out var tokenEl)) return false;

        var token = tokenEl.GetString()!;
        var expiresIn = root.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 3600;
        var expiry = DateTimeOffset.UtcNow.AddSeconds(expiresIn);

        await SecureStorage.Default.SetAsync(TokenKey, token);
        await SecureStorage.Default.SetAsync(TokenExpiryKey, expiry.ToString("O"));

        if (root.TryGetProperty("refresh_token", out var rt))
            await SecureStorage.Default.SetAsync(RefreshTokenKey, rt.GetString()!);

        return true;
    }

    private static string GenerateCodeVerifier()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string GenerateCodeChallenge(string verifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(hash)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
