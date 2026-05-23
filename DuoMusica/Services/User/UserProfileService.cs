using Lyngua.Data.Repositories;
using Lyngua.Helpers;
using Lyngua.Models;

namespace Lyngua.Services.User;

public class UserProfileService : IUserProfileService
{
    private readonly UserRepository _repo;
    private UserProfile? _cache;

    public UserProfileService(UserRepository repo) => _repo = repo;

    public async Task<UserProfile?> GetProfileAsync()
    {
        _cache ??= await _repo.GetActiveProfileAsync();
        return _cache;
    }

    public async Task SaveProfileAsync(UserProfile profile)
    {
        await _repo.SaveProfileAsync(profile);
        _cache = profile;
    }

    public Task<bool> HasProfileAsync() => _repo.HasProfileAsync();

    public async Task AddXpAsync(int xp)
    {
        var profile = await GetProfileAsync();
        if (profile is null) return;

        profile.TotalXp += xp;
        profile.Level = XpCalculator.GetLevel(profile.TotalXp);
        profile.LastPlayedAt = DateTime.UtcNow;
        await _repo.SaveProfileAsync(profile);
    }

    public async Task UpdateAverageWpmAsync(double wpm)
    {
        var profile = await GetProfileAsync();
        if (profile is null) return;

        // Rolling average (weight recent more)
        profile.AverageWpm = profile.AverageWpm * 0.7 + wpm * 0.3;
        await _repo.SaveProfileAsync(profile);
    }
}
