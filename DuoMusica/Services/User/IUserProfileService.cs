using Lyngua.Models;

namespace Lyngua.Services.User;

public interface IUserProfileService
{
    Task<UserProfile?> GetProfileAsync();
    Task SaveProfileAsync(UserProfile profile);
    Task<bool> HasProfileAsync();
    Task AddXpAsync(int xp);
    Task UpdateAverageWpmAsync(double wpm);
}
