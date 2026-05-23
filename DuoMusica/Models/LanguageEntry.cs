namespace Lyngua.Models;

public class LanguageEntry
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public LanguageLevel Level { get; set; }

    public static IReadOnlyList<LanguageEntry> Available => new List<LanguageEntry>
    {
        new() { Code = "en", Name = "English" },
        new() { Code = "es", Name = "Español" },
        new() { Code = "pt", Name = "Português" },
        new() { Code = "fr", Name = "Français" },
        new() { Code = "de", Name = "Deutsch" },
        new() { Code = "it", Name = "Italiano" },
        new() { Code = "ja", Name = "日本語" },
        new() { Code = "ko", Name = "한국어" },
        new() { Code = "zh", Name = "中文" },
        new() { Code = "ar", Name = "العربية" },
    };
}
