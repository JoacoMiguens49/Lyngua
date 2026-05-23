using Lyngua.Data.Repositories;
using Lyngua.Helpers;
using Lyngua.Models;
using Lyngua.Services.Difficulty;
using Lyngua.Services.Lyrics;
using Lyngua.Services.Spotify;
using Lyngua.Services.User;

namespace Lyngua.Services.Game;

public class GameService : IGameService
{
    private readonly ILyricsService _lyrics;
    private readonly IDifficultyService _difficulty;
    private readonly ISpotifyWebApiService _spotifyApi;
    private readonly IUserProfileService _userProfile;
    private readonly ProgressRepository _progress;

    public GameService(
        ILyricsService lyrics,
        IDifficultyService difficulty,
        ISpotifyWebApiService spotifyApi,
        IUserProfileService userProfile,
        ProgressRepository progress)
    {
        _lyrics = lyrics;
        _difficulty = difficulty;
        _spotifyApi = spotifyApi;
        _userProfile = userProfile;
        _progress = progress;
    }

    public async Task<GameSession> BuildSessionAsync(Song song, GameMode mode)
    {
        var lines = await _lyrics.GetSyncedLyricsAsync(song.Name, song.Artist, song.Isrc, song.DurationMs);
        if (lines is null or { Count: 0 })
            throw new InvalidOperationException("No se encontraron letras sincronizadas para esta canción.");

        var segments = SegmentSplitter.Split(lines, song.DurationMs);

        if (song.TempoBeatsPerMinute == 0)
        {
            var tempo = await _spotifyApi.GetTrackTempoAsync(song.SpotifyTrackId);
            song.TempoBeatsPerMinute = tempo ?? 120;
        }

        song.Difficulty = _difficulty.Calculate(segments, song.TempoBeatsPerMinute);

        return new GameSession { Song = song, Mode = mode, Segments = segments };
    }

    public async Task<SegmentResult> EvaluateSegmentAsync(GameSession session, string userInput)
    {
        var segment = session.CurrentSegment
            ?? throw new InvalidOperationException("Session already complete.");

        var correctText = segment.FullText;
        var (tokens, score) = TextDiffHelper.Compare(userInput, correctText);

        var profile = await _userProfile.GetProfileAsync();
        var isFirst = true;

        if (profile is not null)
        {
            var existing = await _progress.GetSegmentProgressAsync(
                profile.Id, session.Song.SpotifyTrackId, segment.Index);
            isFirst = existing is null;
        }

        var isPerfect = score >= 100;
        var xp = XpCalculator.CalculateSegmentXp(score, isFirst, isPerfect);

        var result = new SegmentResult
        {
            SegmentIndex = segment.Index,
            UserInput = userInput,
            CorrectText = correctText,
            Score = score,
            XpEarned = xp,
            DiffTokens = tokens,
            IsFirstAttempt = isFirst
        };

        session.Results.Add(result);
        return result;
    }

    public async Task AdvanceSegmentAsync(GameSession session)
    {
        var profile = await _userProfile.GetProfileAsync();
        if (profile is null) return;

        var lastResult = session.Results.LastOrDefault();
        if (lastResult is not null)
        {
            await SaveSegmentProgressAsync(profile, session, lastResult);
            await _userProfile.AddXpAsync(lastResult.XpEarned);
        }

        session.CurrentSegmentIndex++;
    }

    public async Task FinalizeSessionAsync(GameSession session)
    {
        var profile = await _userProfile.GetProfileAsync();
        if (profile is null) return;

        var existing = await _progress.GetSongProgressAsync(profile.Id, session.Song.SpotifyTrackId);
        var isComplete = session.IsComplete;

        var songProgress = existing ?? new SongProgress
        {
            UserId = profile.Id,
            SpotifyTrackId = session.Song.SpotifyTrackId,
            TrackName = session.Song.Name,
            Artist = session.Song.Artist,
            Language = session.Song.Language,
            TotalSegments = session.Segments.Count
        };

        songProgress.SegmentsCompleted = session.Segments.Count;
        songProgress.BestScore = Math.Max(songProgress.BestScore, session.TotalScore);
        songProgress.LastPlayedAt = DateTime.UtcNow;

        if (isComplete && !songProgress.IsCompleted)
        {
            songProgress.IsCompleted = true;
            songProgress.CompletedAt = DateTime.UtcNow;
            profile.SongsCompleted++;
            await _userProfile.SaveProfileAsync(profile);
        }

        await _progress.SaveSongProgressAsync(songProgress);
    }

    private async Task SaveSegmentProgressAsync(
        Models.UserProfile profile, GameSession session, SegmentResult result)
    {
        var existing = await _progress.GetSegmentProgressAsync(
            profile.Id, session.Song.SpotifyTrackId, result.SegmentIndex);

        var seg = existing ?? new SegmentProgress
        {
            UserId = profile.Id,
            SpotifyTrackId = session.Song.SpotifyTrackId,
            SegmentIndex = result.SegmentIndex
        };

        seg.AttemptCount++;
        seg.BestScore = Math.Max(seg.BestScore, result.Score);
        seg.LastAttemptAt = DateTime.UtcNow;

        await _progress.SaveSegmentProgressAsync(seg);
    }
}
