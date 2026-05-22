using SQLite;

namespace DuoMusica.Models;

[Table("UserProfile")]
public class UserProfile
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    // Stored as JSON: [{"Code":"en","Name":"English","Level":2}]
    public string NativeLanguagesJson { get; set; } = "[]";
    public string PracticeLanguagesJson { get; set; } = "[]";

    public int TotalXp { get; set; }
    public int Level { get; set; } = 1;
    public int SongsCompleted { get; set; }
    public int CurrentStreak { get; set; }
    public double AverageWpm { get; set; } = 40;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastPlayedAt { get; set; } = DateTime.UtcNow;
}
