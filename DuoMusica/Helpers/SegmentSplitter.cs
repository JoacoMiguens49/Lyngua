using Lyngua.Models;

namespace Lyngua.Helpers;

public static class SegmentSplitter
{
    private const int MinDurationMs = 12_000;
    private const int TargetDurationMs = 17_000;
    private const int MaxDurationMs = 22_000;

    public static List<SongSegment> Split(List<LyricLine> lines, int songDurationMs)
    {
        var segments = new List<SongSegment>();
        if (lines.Count == 0) return segments;

        var current = new SongSegment { Index = 0, StartMs = lines[0].TimeMs };

        foreach (var line in lines)
        {
            current.Lines.Add(line);
            var elapsed = line.TimeMs - current.StartMs;

            if (elapsed >= TargetDurationMs)
            {
                // Close segment: end = start of next line (or +2s buffer)
                var nextIndex = lines.IndexOf(line) + 1;
                current.EndMs = nextIndex < lines.Count
                    ? lines[nextIndex].TimeMs
                    : line.TimeMs + 2_000;

                // Only keep segments with some content
                if (current.Lines.Count > 0 && current.DurationMs >= MinDurationMs)
                {
                    segments.Add(current);
                    current = new SongSegment
                    {
                        Index = segments.Count,
                        StartMs = current.EndMs
                    };
                }
            }
        }

        // Flush last segment
        if (current.Lines.Count > 0)
        {
            current.EndMs = songDurationMs > 0 ? songDurationMs : current.Lines.Last().TimeMs + 3_000;
            if (current.DurationMs >= MinDurationMs)
                segments.Add(current);
        }

        // Re-index
        for (int i = 0; i < segments.Count; i++)
            segments[i].Index = i;

        return segments;
    }
}
