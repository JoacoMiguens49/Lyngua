namespace Lyngua.Models;

public enum DiffStatus
{
    Correct,
    Close,
    Wrong,
    Missing
}

public class DiffToken
{
    public string Text { get; set; } = string.Empty;
    public DiffStatus Status { get; set; }
}
