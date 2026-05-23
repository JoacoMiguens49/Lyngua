namespace Lyngua.Models;

public class SongSegment
{
    public int Index { get; set; }
    public int StartMs { get; set; }
    public int EndMs { get; set; }
    public List<LyricLine> Lines { get; set; } = new();

    public int DurationMs => EndMs - StartMs;

    public string FullText => string.Join(" ", Lines.Select(l => l.Text)).Trim();
}
