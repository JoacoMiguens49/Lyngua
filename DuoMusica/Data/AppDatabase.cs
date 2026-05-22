using DuoMusica.Models;
using SQLite;

namespace DuoMusica.Data;

public class AppDatabase
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db is not null) return _db;

        var path = Path.Combine(FileSystem.AppDataDirectory, "duomusica.db3");
        _db = new SQLiteAsyncConnection(path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _db.CreateTableAsync<UserProfile>();
        await _db.CreateTableAsync<SongProgress>();
        await _db.CreateTableAsync<SegmentProgress>();

        return _db;
    }

    public async Task<SQLiteAsyncConnection> GetDbAsync() => await GetConnectionAsync();
}
