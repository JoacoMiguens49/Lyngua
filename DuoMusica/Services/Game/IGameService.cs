using Lyngua.Models;

namespace Lyngua.Services.Game;

public interface IGameService
{
    Task<GameSession> BuildSessionAsync(Song song, GameMode mode);
    Task<SegmentResult> EvaluateSegmentAsync(GameSession session, string userInput);
    Task AdvanceSegmentAsync(GameSession session);
    Task FinalizeSessionAsync(GameSession session);
}
