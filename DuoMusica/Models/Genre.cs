namespace DuoMusica.Models;

public class Genre
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SpotifyGenreTag { get; set; } = string.Empty;

    public static IReadOnlyList<Genre> All => new List<Genre>
    {
        new() { Id = "pop",        Name = "Pop",        SpotifyGenreTag = "pop" },
        new() { Id = "rock",       Name = "Rock",       SpotifyGenreTag = "rock" },
        new() { Id = "hiphop",     Name = "Hip-Hop",    SpotifyGenreTag = "hip-hop" },
        new() { Id = "rnb",        Name = "R&B",        SpotifyGenreTag = "r-n-b" },
        new() { Id = "latin",      Name = "Latin",      SpotifyGenreTag = "latin" },
        new() { Id = "electronic", Name = "Electronic", SpotifyGenreTag = "electronic" },
        new() { Id = "jazz",       Name = "Jazz",       SpotifyGenreTag = "jazz" },
        new() { Id = "classical",  Name = "Classical",  SpotifyGenreTag = "classical" },
        new() { Id = "country",    Name = "Country",    SpotifyGenreTag = "country" },
        new() { Id = "reggae",     Name = "Reggae",     SpotifyGenreTag = "reggae" },
    };
}
