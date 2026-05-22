namespace DuoMusica.Services.Spotify;

public interface ISpotifyAuthService
{
    Task<bool> AuthenticateAsync();
    Task<string?> GetAccessTokenAsync();
    Task RevokeAsync();
    bool IsAuthenticated { get; }
}
