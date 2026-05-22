using DuoMusica.Models;

namespace DuoMusica.Data.Repositories;

public class UserRepository
{
    private readonly AppDatabase _db;

    public UserRepository(AppDatabase db) => _db = db;

    public async Task<UserProfile?> GetActiveProfileAsync()
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<UserProfile>().FirstOrDefaultAsync();
    }

    public async Task<int> SaveProfileAsync(UserProfile profile)
    {
        var conn = await _db.GetDbAsync();
        return profile.Id == 0
            ? await conn.InsertAsync(profile)
            : await conn.UpdateAsync(profile);
    }

    public async Task<bool> HasProfileAsync()
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<UserProfile>().CountAsync() > 0;
    }
}
