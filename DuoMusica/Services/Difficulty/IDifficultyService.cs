using DuoMusica.Models;

namespace DuoMusica.Services.Difficulty;

public interface IDifficultyService
{
    DifficultyLevel Calculate(List<SongSegment> segments, int tempoBeatsPerMinute);
}
