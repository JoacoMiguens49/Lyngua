using Lyngua.Models;
using System.Text.RegularExpressions;

namespace Lyngua.Helpers;

public static class LrcParser
{
    // Matches [mm:ss.xx] or [mm:ss:xx]
    private static readonly Regex _timestampRegex =
        new(@"^\[(\d{2}):(\d{2})[\.:](\d{2,3})\](.*)", RegexOptions.Compiled);

    public static List<LyricLine> Parse(string lrcContent)
    {
        var lines = new List<LyricLine>();

        foreach (var raw in lrcContent.Split('\n'))
        {
            var line = raw.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            var match = _timestampRegex.Match(line);
            if (!match.Success) continue;

            var minutes = int.Parse(match.Groups[1].Value);
            var seconds = int.Parse(match.Groups[2].Value);
            var fraction = match.Groups[3].Value;
            var text = match.Groups[4].Value.Trim();

            // Normalize fraction to ms (2 digits = centiseconds, 3 digits = milliseconds)
            var ms = fraction.Length == 3
                ? int.Parse(fraction)
                : int.Parse(fraction) * 10;

            var timeMs = minutes * 60_000 + seconds * 1_000 + ms;

            if (!string.IsNullOrWhiteSpace(text))
                lines.Add(new LyricLine { TimeMs = timeMs, Text = text });
        }

        return lines.OrderBy(l => l.TimeMs).ToList();
    }
}
