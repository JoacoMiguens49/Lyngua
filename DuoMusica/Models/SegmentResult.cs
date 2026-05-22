namespace DuoMusica.Models;

public class SegmentResult
{
    public int SegmentIndex { get; set; }
    public string UserInput { get; set; } = string.Empty;
    public string CorrectText { get; set; } = string.Empty;
    public double Score { get; set; }
    public int XpEarned { get; set; }
    public List<DiffToken> DiffTokens { get; set; } = new();
    public bool IsFirstAttempt { get; set; }
}
