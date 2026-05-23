using Lyngua.Models;

namespace Lyngua.Data.Repositories;

public class ProgressRepository
{
    private readonly AppDatabase _db;

    public ProgressRepository(AppDatabase db) => _db = db;

    public async Task<SongProgress?> GetSongProgressAsync(int userId, string trackId)
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<SongProgress>()
            .Where(p => p.UserId == userId && p.SpotifyTrackId == trackId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<SongProgress>> GetRecentAsync(int userId, int count = 5)
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<SongProgress>()
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.LastPlayedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<int> SaveSongProgressAsync(SongProgress progress)
    {
        var conn = await _db.GetDbAsync();
        return progress.Id == 0
            ? await conn.InsertAsync(progress)
            : await conn.UpdateAsync(progress);
    }

    public async Task<SegmentProgress?> GetSegmentProgressAsync(int userId, string trackId, int segmentIndex)
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<SegmentProgress>()
            .Where(p => p.UserId == userId && p.SpotifyTrackId == trackId && p.SegmentIndex == segmentIndex)
            .FirstOrDefaultAsync();
    }

    public async Task<int> SaveSegmentProgressAsync(SegmentProgress progress)
    {
        var conn = await _db.GetDbAsync();
        return progress.Id == 0
            ? await conn.InsertAsync(progress)
            : await conn.UpdateAsync(progress);
    }

    public async Task<int> GetCompletedSongsCountAsync(int userId)
    {
        var conn = await _db.GetDbAsync();
        return await conn.Table<SongProgress>()
            .Where(p => p.UserId == userId && p.IsCompleted)
            .CountAsync();
    }
}
