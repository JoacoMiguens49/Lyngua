using Lyngua.Models;

namespace Lyngua.Services.Difficulty;

public interface IDifficultyService
{
    DifficultyLevel Calculate(List<SongSegment> segments, int tempoBeatsPerMinute);
}
